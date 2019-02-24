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
            // Find a station
            ConversationInfo info = RingoBotHelper.NormalizedConversationInfo(turnContext);
            Station station = await _ringoService.FindStation(info, query, cancellationToken);

            if (station == null && string.IsNullOrEmpty(query))
            {
                await turnContext.SendActivityAsync(
                    "No stations playing. Would you like to start one? Type `\"play (playlist name)\"`",
                    cancellationToken: cancellationToken);
                return;
            }

            if (station == null && query.StartsWith('@'))
            {
                await CreateAndJoinUserStation(turnContext, query, cancellationToken);
                return;
            }

            if (station == null)
            {
                await turnContext.SendActivityAsync(
                    $"Could not find station \"{query}\" 🤔",
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

            // Join
            await _ringoService.JoinPlaylist(
                turnContext,
                query,
                token.Token,
                station,
                stationToken.Token,
                cancellationToken);
        }

        private async Task CreateAndJoinUserStation(ITurnContext turnContext, string query, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{turnContext.Activity.From.Name} has asked to create and join user station {query}");

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
            await CreateStation(turnContext, channelUserId, joinUserToken.Token, playlist, null, cancellationToken);
            return;

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
            token = token ?? await _authService.Authorize(turnContext, cancellationToken);

            if (token == null)
            {
                // resume after auth
                userProfile.ResumeAfterAuthorizationWith = (PlayCommand[0], query);
                return;
            }

            Playlist playlist = null;
            string hashtag = null;

            if (string.IsNullOrEmpty(query))
            {
                // no query so start / resume station
                playlist = await GetNowPlaying(token.Token);

                if (playlist == null)
                {
                    // user is not playing a playlist
                    await turnContext.SendActivityAsync(
                        "You are not currently playing a Spotify Playlist.",
                        cancellationToken: cancellationToken);

                    // TODO: What would you like to play?
                    return;
                }
            }
            else
            {
                // playlist query
                string search = null;

                //if (query.StartsWith("album ", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    //XXX replace album text...
                //    string album = query.Substring(6).Trim();

                //    if (album.Length == 0)
                //    {
                //        await turnContext.SendActivityAsync(
                //            "I did not understand. Try `\" \"`",
                //            cancellationToken: cancellationToken);

                //    }

                //    await _ringoService.PlayAlbum(
                //        turnContext,
                //        query.Substring(X, X),
                //        token.Token,
                //        cancellationToken);
                //}

                if (query.Contains('#'))
                {
                    search = query.Substring(0, query.IndexOf('#'));
                    hashtag = query.Substring(query.IndexOf('#') + 1);
                }
                else
                {
                    search = query;
                }

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
                    playlist, 
                    $"{RingoBotHelper.RingoHandleIfGroupChat(turnContext)}play", 
                    cancellationToken))
                {
                    return;
                }

                await _ringoService.PlayPlaylist(
                    playlist.Id,
                    token.Token,
                    cancellationToken);
            }

            if (playlist == null) return;

            await CreateStation(turnContext,
                RingoBotHelper.ChannelUserId(turnContext),
                token.Token,
                playlist,
                hashtag,
                cancellationToken);
        }

        private async Task<bool> IsDeviceActive(
            ITurnContext turnContext,
            string token,
            Playlist playlist,
            string commandQuery,
            CancellationToken cancellationToken)
        {
            var devices = await _ringoService.GetDevices(token);

            if (devices.Any(d => d.IsActive)) return true;

            // No active devices
            var heroCard = new HeroCard
            {
                Text = "Open Spotify and click/tap Play",
                Buttons = new[]
                {
                    new CardAction
                    {
                        Title = "Open Spotify",
                        Text = "Open the Spotify Player and click/tap Play to make Spotify active",
                        Value = playlist.Uri,
                        Type = ActionTypes.OpenUrl,
                    },
                },
            };

            if (playlist.Images.Any())
            {
                heroCard.Images = new[]
                {
                    new CardImage
                    {
                        Url = playlist.Images[0].Url,
                        Alt = playlist.Name,
                        Tap = heroCard.Buttons[0]
                    }
                };
            }

            var attachment = new Attachment
            {
                ContentType = HeroCard.ContentType,
                Content = heroCard
            };

            var message = MessageFactory.Attachment(
                attachment, 
                text: "Ringo can't see any active Spotify devices. Click the button below to open Spotify and then press play. Once Spotify is playing, click/tap Try Again.");
                
            message.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Try Again", Type = ActionTypes.ImBack, Value = commandQuery },
                },
            };

            await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);

            return false;
        }

        private async Task CreateStation(
            ITurnContext turnContext,
            string channelUserId,
            string token,
            Playlist playlist,
            string hashtag,
            CancellationToken cancellationToken)
        {
            var station = await _ringoService.CreateStation(
                channelUserId,
                RingoBotHelper.NormalizedConversationInfo(turnContext),
                playlist,
                hashtag);

            var heroCard = new HeroCard
            {
                Text = $"#{station.Hashtag}"
            };

            if (playlist.Images.Any())
            {
                heroCard.Images = new[]
                {
                    new CardImage
                    {
                        Url = playlist.Images[0].Url,
                        Alt = playlist.Name
                    }
                };
            }

            var attachment = new Attachment
            {
                ContentType = HeroCard.ContentType,
                Content = heroCard
            };

            if (BotHelper.IsGroup(turnContext))
            {
                var message = MessageFactory.Attachment(
                    attachment,
                    text: $"{turnContext.Activity.From.Name} is playing \"{station.Name}\" #{station.Hashtag}. Type `\"@ringo join\"` to join in! 🎉");

                await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);
            }
            else
            {
                var message = MessageFactory.Attachment(
                    attachment,
                    text: $"Now playing \"{station.Name}\" #{station.Hashtag}. Friends can type `\"join #{station.Hashtag}\"` to join in! 🎉");

                await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);
            }
        }
    }
}
