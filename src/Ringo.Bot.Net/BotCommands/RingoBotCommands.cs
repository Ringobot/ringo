using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using RingoBotNet.Services;
using RingoBotNet.State;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet
{
    public class RingoBotCommands : IRingoBotCommands
    {
        internal static string[] AuthCommand = new[] { "auth", "authorize" };
        internal static string[] JoinCommand = new[] { "join", "listen" };
        internal static string[] PlayCommand = new[] { "start", "play" };

        private readonly IAuthService _authService;
        private readonly IRingoService _ringoService;

        public RingoBotCommands(IAuthService authService, IRingoService ringoService)
        {
            _authService = authService;
            _ringoService = ringoService;
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
            Station station = await _ringoService.FindStation(turnContext, query, cancellationToken);

            if (station == null && string.IsNullOrEmpty(query))
            {
                await turnContext.SendActivityAsync(
                    "No stations playing. Would you like to start one? Type `\"play (playlist name)\"`",
                    cancellationToken: cancellationToken);
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

        public async Task Play(
            ITurnContext turnContext,
            UserProfile userProfile,
            string query,
            CancellationToken cancellationToken,
            TokenResponse token = null
            )
        {
            token = token ?? await _authService.Authorize(turnContext, cancellationToken);

            if (token == null)
            {
                // resume after auth
                userProfile.ResumeAfterAuthorizationWith = (PlayCommand[0], query);
                return;
            }

            if (string.IsNullOrEmpty(query)) return;
            //{
            //    await _ringoService.Start(
            //        turnContext,
            //        token.Token,
            //        cancellationToken);
            //      return;
            //}

            //if (query.StartsWith("album ", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    XXX replace album text...
            //    await _ringoService.PlayAlbum(
            //        turnContext,
            //        query.Substring(X,X),
            //        token.Token,
            //        cancellationToken);
            //}

            string search = null;
            string hashtag = null;

            if (query.Contains('#'))
            {
                search = query.Substring(0, query.IndexOf('#'));
                hashtag = query.Substring(query.IndexOf('#') + 1);
            }
            else
            {
                search = query;
            }

            var playlist = await _ringoService.PlayPlaylist(
                turnContext,
                query,
                token.Token,
                cancellationToken);

            if (playlist == null) return;

            var station = await _ringoService.CreateStation(turnContext, playlist, cancellationToken, hashtag);

            //await turnContext.SendActivityAsync(
            //    $"Tell your friends to type `join {hashcode}` into Ringobot to join the party! 🥳",
            //    cancellationToken: cancellationToken);

            if (BotHelper.IsGroup(turnContext))
            {
                await turnContext.SendActivityAsync(
                        $"{turnContext.Activity.From.Name} is playing \"{station.Name}\" #{station.Hashtag}. Type `\"join\"` to join the party! 🎉",
                        cancellationToken: cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(
                    $"Now playing \"{station.Name}\" #{station.Hashtag}. Friends can type `\"join\"` to join the party! 🎉",
                    cancellationToken: cancellationToken);
            }
        }
    }
}
