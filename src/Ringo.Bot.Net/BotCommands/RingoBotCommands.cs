using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using RingoBotNet.Helpers;
using RingoBotNet.Services;
using RingoBotNet.State;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet
{
    public class RingoBotCommands : IRingoBotCommands
    {
        internal const string AuthCommand = "auth";
        internal const string AuthCommandAlias = "authorize";
        internal const string JoinCommand = "join";
        internal const string PlayCommand = "play";

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

                if (userProfile.ResumeAfterAuthorizationWith.command == PlayCommand)
                {
                    await Play(
                        turnContext,
                        userProfile,
                        userProfile.ResumeAfterAuthorizationWith.query,
                        cancellationToken,
                        token: token);
                }
                else if (userProfile.ResumeAfterAuthorizationWith.command == JoinCommand)
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
            TokenResponse token = null
            )
        {
            // get mentioned user and their token
            ChannelAccount mentioned = BotHelper.GetFirstMentioned(turnContext);

            if (mentioned == null)
            {
                await turnContext.SendActivityAsync(
                    $"I did not understand 🤔 Try mentioning another user, e.g. `\"{RingoService.RingoHandleIfGroupChat(turnContext)}join @username\"`",
                    cancellationToken: cancellationToken);
                return;
            }

            TokenResponse mentionedToken = await _authService.GetAccessToken(turnContext.Activity.ChannelId, mentioned.Id);

            if (mentionedToken == null)
            {
                await turnContext.SendActivityAsync(
                    $"Join failed. @{mentioned.Name} has not asked Ringo to play anything 🤨 Type `\"{RingoService.RingoHandleIfGroupChat(turnContext)}play (playlist name)\"` to get started.",
                    cancellationToken: cancellationToken);
                return;
            }

            // get user and their token
            token = token ?? await _authService.Authorize(turnContext, cancellationToken);

            if (token == null)
            {
                // resume after auth
                userProfile.ResumeAfterAuthorizationWith = (JoinCommand, query);
                return;
            }

            // Join
            await _ringoService.JoinPlaylist(
                turnContext,
                query,
                token.Token,
                mentioned,
                mentionedToken.Token,
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
                userProfile.ResumeAfterAuthorizationWith = (PlayCommand, query);
                return;
            }

            await _ringoService.PlayPlaylist(
                turnContext,
                query,
                token.Token,
                cancellationToken);
        }
    }
}
