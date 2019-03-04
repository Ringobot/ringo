using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using RingoBotNet.Services;
using RingoBotNet.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet
{
    public class RingoBotCommands : IRingoBotCommands
    {
        internal static string[] AuthCommand = new[] { "auth", "authorize" };
        internal static string[] JoinCommand = new[] { "join", "listen", "sync" };
        internal static string[] PlayCommand = new[] { "start", "play" };

        private readonly IAuthService _authService;
        private readonly IRingoService _ringoService;
        private readonly ILogger _logger;

        public RingoBotCommands(IAuthService authService, IRingoService ringoService, ILogger<RingoBotCommands> logger)
        {
            _authService = authService;
            _ringoService = ringoService;
            _logger = logger;
        }

        public async Task Auth(ITurnContext turnContext, UserProfile userProfile, string query, CancellationToken cancellationToken)
        {
            // Don't auth in group chat
            if (BotHelper.IsGroup(turnContext))
            {
                await turnContext.SendActivityAsync(
                    "Ringo cannot authorize you in Group Chat. DM (direct message) @ringo instead.",
                    cancellationToken: cancellationToken);
                return;
            }

            // RESET
            if (query.ToLower() == "reset")
            {
                await _authService.ResetAuthorization(turnContext, cancellationToken);
                await turnContext.SendActivityAsync(
                    "Spotify authorization has been reset. Type `\"authorize\"` to authorize Spotify again.",
                    cancellationToken: cancellationToken);
                return;
            }

            // MAGIC NUMBER
            if (AuthService.RingoBotStateRegex.IsMatch(query))
            {
                var token = await _authService.ValidateMagicNumber(turnContext, query, cancellationToken);

                if (PlayCommand.Contains(userProfile.ResumeAfterAuthorizationWith.command))
                {
                    await Play(
                        turnContext,
                        userProfile,
                        userProfile.ResumeAfterAuthorizationWith.query,
                        cancellationToken,
                        token: token);
                }
                else if (JoinCommand.Contains(userProfile.ResumeAfterAuthorizationWith.command))
                {
                    await Join(
                        turnContext,
                        userProfile,
                        userProfile.ResumeAfterAuthorizationWith.query,
                        cancellationToken,
                        token: token);
                }

                return;
            }

            // AUTH
            var token2 = await _authService.Authorize(turnContext, cancellationToken);

            if (token2 == null)
            {
                // clear resume after auth
                userProfile.ResumeAfterAuthorizationWith = (null, null);
                return;
            }

            await turnContext.SendActivityAsync(
                "Ringo is authorized to play Spotify. Ready to rock! 😎",
                cancellationToken: cancellationToken);
        }

        public async Task Join(
            ITurnContext turnContext,
            UserProfile userProfile,
            string query,
            CancellationToken cancellationToken,
            TokenResponse token = null)
        {
            ConversationInfo info = RingoBotHelper.NormalizedConversationInfo(turnContext);

            if (!info.IsGroup && (string.IsNullOrEmpty(query) || (!query.StartsWith('#') && !query.StartsWith('@'))))
            {
                // must specify #channel or @username in DM to join a Station
                await turnContext.SendActivityAsync(
                    RingoBotMessages.JoinWhat(),
                    cancellationToken: cancellationToken);
                return;
            }

            Station station = null;

            if (string.IsNullOrEmpty(query))
            {
                // just `join` in group chat: Play the current channel station
                station = await _ringoService.GetChannelStation(info);
            }
            else if (query.StartsWith('@'))
            {
                // user station
                station = await _ringoService.GetUserStation(info, query.Substring(1));
            }
            else if (query.StartsWith('#'))
            {
                // could either be a channel station or hashtag station
                station = await _ringoService.GetChannelStation(info, query.Substring(1));
            }

            if (station == null && query.StartsWith('@'))
            {
                await CreateAndJoinUserStation(turnContext, query, cancellationToken);
                return;
            }

            if (station == null)
            {
                await turnContext.SendActivityAsync(
                    RingoBotMessages.CouldNotFindStation(info, query),
                    cancellationToken: cancellationToken);
                return;
            }

            TokenResponse stationToken = await _authService.GetAccessToken(station.ChannelUserId);

            // get user and their token
            token = token ?? await _authService.Authorize(turnContext, cancellationToken);

            if (token == null)
            {
                // resume after auth
                userProfile.ResumeAfterAuthorizationWith = (JoinCommand[0], query);
                return;
            }

            if (!await IsDeviceActive(
                turnContext,
                token.Token,
                $"{RingoBotHelper.RingoHandleIfGroupChat(turnContext)}join {query}",
                cancellationToken,
                station.Playlist))
            {
                return;
            }

            // Join
            try
            {
                if (await _ringoService.JoinPlaylist(
                    query,
                    token.Token,
                    station,
                    stationToken.Token,
                    cancellationToken))
                {
                    await turnContext.SendActivityAsync(
                        RingoBotMessages.UserHasJoined(info, station),
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(
                        RingoBotMessages.StationNoLongerPlaying(info, station),
                        cancellationToken: cancellationToken);
                }
            }
            catch (SpotifyApi.NetCore.SpotifyApiErrorException ex)
            {
                await turnContext.SendActivityAsync(ex.Message, cancellationToken: cancellationToken);
            }
        }

        private async Task CreateAndJoinUserStation(ITurnContext turnContext, string query, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{turnContext.Activity.From.Name} has asked to create and join user station {query}");

            var info = RingoBotHelper.NormalizedConversationInfo(turnContext);

            // Join with user who is playing but not created/owned a Station
            string username = query.Substring(1).ToLower();
            string channelUserId = RingoBotHelper.ChannelUserId(turnContext.Activity.ChannelId, username);

            TokenResponse joinUserToken = await _authService.GetAccessToken(channelUserId);
            if (joinUserToken == null)
            {
                _logger.LogWarning($"Could not find join user token for user `{username}`");
                // join user does not have a current access token
                await turnContext.SendActivityAsync(
                    $"Ringo bot is not authorized to join {query} 😢",
                    cancellationToken: cancellationToken);
                return;
            }

            var playlist = await GetNowPlaying(joinUserToken.Token);

            if (playlist == null)
            {
                // join user is not playing a playlist
                await turnContext.SendActivityAsync(
                    $"{query} is not currently playing a Spotify Playlist.",
                    cancellationToken: cancellationToken);

                // TODO: What would you like to play?
                return;
            }

            // Create station
            var station = await _ringoService.CreateUserStation(channelUserId, info, playlist);
            await turnContext.SendActivityAsync(RingoBotMessages.NowPlayingStation(info, station), cancellationToken: cancellationToken);
        }

        private async Task<Playlist> GetNowPlaying(string token)
        {
            SpotifyApi.NetCore.CurrentPlaybackContext nowPlaying = await _ringoService.GetUserNowPlaying(token);

            if (nowPlaying == null)
            {
                return null;
            }

            // Get the playlist that is now playing
            return await _ringoService.GetPlaylist(token, nowPlaying.Context.Uri);
        }

        public async Task Play(
            ITurnContext turnContext,
            UserProfile userProfile,
            string query,
            CancellationToken cancellationToken,
            TokenResponse token = null)
        {
            var info = RingoBotHelper.NormalizedConversationInfo(turnContext);

            token = token ?? await _authService.Authorize(turnContext, cancellationToken);

            if (token == null)
            {
                // resume after auth
                userProfile.ResumeAfterAuthorizationWith = (PlayCommand[0], query);
                return;
            }

            Playlist playlist = null;
            //string hashtag = null;

            if (string.IsNullOrEmpty(query))
            {
                // Play whatever the user is currently playing on Spotify
                if (!await IsDeviceActive(
                    turnContext, 
                    token.Token, 
                    $"{RingoBotHelper.RingoHandleIfGroupChat(turnContext)}play", 
                    cancellationToken))
                {
                    return;
                }

                // no query so start / resume station
                playlist = await GetNowPlaying(token.Token);

                if (playlist == null)
                {
                    // user is not playing a playlist
                    await turnContext.SendActivityAsync(
                        "You are not currently playing a Spotify Playlist.",
                        cancellationToken: cancellationToken);
                    return;
                }
            }
            else
            {
                // Search for a Playlist and command Spotify to Play it

                // playlist query
                string search = null;

                //if (query.Contains('#'))
                //{
                //    search = query.Substring(0, query.IndexOf('#'));
                //    hashtag = query.Substring(query.IndexOf('#') + 1);
                //}
                //else
                //{
                //    search = query;
                //}

                search = query;

                Playlist[] playlists = await _ringoService.FindPlaylists(
                    search,
                    token.Token,
                    cancellationToken);

                if (playlists == null || !playlists.Any())
                {
                    await turnContext.SendActivityAsync($"No playlists found!", cancellationToken: cancellationToken);
                    return;
                }
                
                // TODO: Carousel
                playlist = playlists[0];

                if (!await IsDeviceActive(
                    turnContext, 
                    token.Token, 
                    $"{RingoBotHelper.RingoHandleIfGroupChat(turnContext)}play {playlist.Uri}", 
                    cancellationToken,
                    playlist: playlist))
                {
                    return;
                }

                // Command Spotify to play the Playlist
                await _ringoService.PlayPlaylist(
                    playlist.Id,
                    token.Token,
                    cancellationToken);
            }

            if (playlist == null) return;

            // Playlist is playing, now create a station
            var station = info.IsGroup
                ? await _ringoService.CreateChannelStation(RingoBotHelper.ChannelUserId(turnContext), info, playlist)
                : await _ringoService.CreateUserStation(RingoBotHelper.ChannelUserId(turnContext), info, playlist);
            await turnContext.SendActivityAsync(RingoBotMessages.NowPlayingStation(info, station), cancellationToken);
        }

        private async Task<bool> IsDeviceActive(
            ITurnContext turnContext,
            string token,
            string commandQuery,
            CancellationToken cancellationToken,
            Playlist playlist = null)
        {
            var devices = await _ringoService.GetDevices(token);

            if (devices.Any(d => d.IsActive)) return true;

            // No active devices

            var heroCard = new HeroCard { Buttons = new List<CardAction>() };

            if (playlist != null)
            {
                heroCard.Buttons.Add(
                    new CardAction
                    {
                        Title = "Open Spotify",
                        Value = playlist.Uri,
                        Type = ActionTypes.OpenUrl,
                    });
                heroCard.Buttons.Add(
                    new CardAction
                    {
                        Title = "Open Spotify Web Player",
                        Value = playlist.ExternalUrls.Spotify,
                        Type = ActionTypes.OpenUrl,
                    });
            }

            heroCard.Buttons.Add(
                new CardAction
                {
                    Title = "Spotify is playing! Try Again",
                    Type = ActionTypes.ImBack,
                    Value = commandQuery
                });

            var message = MessageFactory.Attachment(
                new Attachment { ContentType = HeroCard.ContentType, Content = heroCard }, 
                text: "To play or join with Ringo your Spotify app needs to be active or playing. Open Spotify and press play, then try again.");

            await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);

            return false;
        }
    }
}
