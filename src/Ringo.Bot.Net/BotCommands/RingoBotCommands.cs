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
        private readonly ISpotifyService _spotifyService;
        private readonly ILogger _logger;

        public RingoBotCommands(
            IAuthService authService,
            IRingoService ringoService,
            ISpotifyService spotifyService,
            ILogger<RingoBotCommands> logger)
        {
            _authService = authService;
            _ringoService = ringoService;
            _spotifyService = spotifyService;
            _logger = logger;
        }

        public async Task Auth(ITurnContext turnContext, UserProfile userProfile, string query, CancellationToken cancellationToken)
        {
            var info = RingoBotHelper.NormalizedConversationInfo(turnContext);

            // Don't auth in group chat
            if (BotHelper.IsGroup(turnContext))
            {
                await turnContext.SendActivityAsync(
                    $"Ringo cannot authorize you in Group Chat. DM (direct message) @{info.BotName} instead.",
                    cancellationToken: cancellationToken);
                return;
            }

            // RESET
            if (query.ToLower() == "reset")
            {
                await _authService.ResetAuthorization(turnContext, cancellationToken);
                await turnContext.SendActivityAsync(RingoBotMessages.AuthHasBeenReset(info), cancellationToken);
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

            Station2 station = null;

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

            //if (station == null && query.StartsWith('@'))
            //{
            //    await CreateAndJoinUserStation(turnContext, query, cancellationToken);
            //    return;
            //}

            if (station == null)
            {
                await turnContext.SendActivityAsync(
                    RingoBotMessages.CouldNotFindStation(info, query),
                    cancellationToken: cancellationToken);
                return;
            }

            TokenResponse stationToken = await _authService.GetAccessToken(station.Owner.UserId);

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
                if (await _spotifyService.JoinPlaylist(
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
                await turnContext.SendActivityAsync(RingoBotMessages.SpotifyError(info, ex, $"join {query}"), cancellationToken: cancellationToken);
            }
        }

        public async Task Play(
            ITurnContext turnContext,
            UserProfile userProfile,
            string query,
            CancellationToken cancellationToken,
            TokenResponse token = null)
        {
            var info = RingoBotHelper.NormalizedConversationInfo(turnContext);
            string channelUserId = RingoBotHelper.ChannelUserId(turnContext);

            // User authorised?
            token = token ?? await _authService.Authorize(turnContext, cancellationToken);

            if (token == null)
            {
                // resume after auth
                userProfile.ResumeAfterAuthorizationWith = (PlayCommand[0], query);
                return;
            }

            // Device active?
            if (!await IsDeviceActive(
                turnContext,
                token.Token,
                $"{RingoBotHelper.RingoHandleIfGroupChat(turnContext)}play {query}",
                cancellationToken))
            {
                return;
            }

            Station2 station = null;

            if (string.IsNullOrEmpty(query))
            {
                // Play whatever is now playing on Spotify
                station = await PlayNowPlaying(turnContext, token.Token, cancellationToken);
                if (station == null) return;
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

                Playlist[] playlists = await _spotifyService.FindPlaylists(
                    search,
                    token.Token,
                    cancellationToken);

                if (playlists == null || !playlists.Any())
                {
                    await turnContext.SendActivityAsync($"No playlists found!", cancellationToken: cancellationToken);
                    return;
                }
                
                // TODO: Carousel
                Playlist playlist = playlists[0];

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
                await _spotifyService.PlayPlaylist(
                    playlist.Id,
                    token.Token,
                    cancellationToken);

                if (playlist == null) return;

                // Playlist is playing, now create a station
                station = info.IsGroup
                    ? await _ringoService.CreateChannelStation(channelUserId, info, playlist: playlist)
                    : await _ringoService.CreateUserStation(channelUserId, info, playlist: playlist);

            }

            await turnContext.SendActivityAsync(RingoBotMessages.NowPlayingStation(info, station), cancellationToken);
        }

        private async Task<Station2> PlayNowPlaying(ITurnContext turnContext, string token, CancellationToken cancellationToken)
        {
            // no query so start / resume station
            // Play whatever the user is currently playing on Spotify

            var info = RingoBotHelper.NormalizedConversationInfo(turnContext);
            var channelUserId = RingoBotHelper.ChannelUserId(turnContext);

            SpotifyApi.NetCore.CurrentPlaybackContext nowPlaying = await _spotifyService.GetUserNowPlaying(token);

            if (nowPlaying == null || !nowPlaying.IsPlaying || nowPlaying.Context == null)
            {
                if (nowPlaying.IsPlaying && nowPlaying.Context == null)
                {
                    await turnContext.SendActivityAsync(RingoBotMessages.NowPlayingNoContext(info), cancellationToken);
                }
                else if (nowPlaying.IsPlaying && !new[] { "playlist", "album" }.Contains(nowPlaying.Context.Type))
                {
                    await turnContext.SendActivityAsync(
                        RingoBotMessages.NowPlayingNotSupported(info, nowPlaying.Context.Type), cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(RingoBotMessages.NotPlayingAnything(info), cancellationToken);
                }
                
                return null;
            }

            await _spotifyService.TurnOffShuffleRepeat(token, nowPlaying);

            Station2 station = null;

            switch (nowPlaying.Context.Type)
            {
                case "playlist":
                    var playlist = await _spotifyService.GetPlaylist(token, nowPlaying.Context.Uri);
                    station = info.IsGroup
                        ? await _ringoService.CreateChannelStation(channelUserId, info, playlist:playlist)
                        : await _ringoService.CreateUserStation(channelUserId, info, playlist:playlist);
                    break;

                case "album":
                    var album = await _spotifyService.GetAlbum(token, nowPlaying.Context.Uri);
                    station = info.IsGroup
                        ? await _ringoService.CreateChannelStation(channelUserId, info, album: album)
                        : await _ringoService.CreateUserStation(channelUserId, info, album: album);
                    break;

                default:
                    await turnContext.SendActivityAsync(
                        RingoBotMessages.NowPlayingNotSupported(info, nowPlaying.Context.Type), cancellationToken);
                    break;
            }

            return station;
        }

        private async Task<bool> IsDeviceActive(
            ITurnContext turnContext,
            string token,
            string commandQuery,
            CancellationToken cancellationToken,
            Playlist playlist = null)
        {
            var devices = await _spotifyService.GetDevices(token);

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
