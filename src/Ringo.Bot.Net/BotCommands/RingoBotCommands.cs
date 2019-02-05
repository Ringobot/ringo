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
        internal const string JoinCommand = "join";
        private readonly IAuthService _authService;
        private readonly IRingoService _ringoService;

        public RingoBotCommands(IAuthService authService, IRingoService ringoService)
        {
            _authService = authService;
            _ringoService = ringoService;
        }

        public async Task Join(ITurnContext turnContext, UserProfile userProfile, string query, CancellationToken cancellationToken)
        {
            // get mentioned user and their token
            ChannelAccount mentioned = BotHelpers.GetFirstMentioned(turnContext);

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
            var token2 = await _authService.Authorize(turnContext, cancellationToken);

            if (token2 == null)
            {
                // resume after auth
                userProfile.ResumeAfterAuthorizationWith = (JoinCommand, query);
                return;
            }

            // Join
            await _ringoService.JoinPlaylist(
                turnContext,
                query,
                token2.Token,
                mentioned,
                mentionedToken.Token,
                cancellationToken);
        }
    }
}
