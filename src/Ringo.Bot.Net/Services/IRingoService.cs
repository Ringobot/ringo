using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Ringo.Bot.Net.State;

namespace Ringo.Bot.Net.Services
{
    public interface IRingoService
    {
        Task PlayPlaylist(ITurnContext turnContext, string searchText, string accessToken, CancellationToken cancellationToken);

        Task JoinPlaylist(ITurnContext turnContext, string joinUsername, ConversationData conversationData, string token, CancellationToken cancellationToken);

        Task<TokenResponse> Authorize(
            ITurnContext turnContext,
            string userName,
            ConversationData conversationData,
            CancellationToken cancellationToken);
    }
}