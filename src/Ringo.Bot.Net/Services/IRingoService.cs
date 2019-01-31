using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using RingoBotNet.State;

namespace RingoBotNet.Services
{
    public interface IRingoService
    {
        Task PlayPlaylist(ITurnContext turnContext, string searchText, string accessToken, CancellationToken cancellationToken);

        Task JoinPlaylist(ITurnContext turnContext, string joinUsername, ConversationData conversationData, string token, CancellationToken cancellationToken);

        Task<TokenResponse> Authorize(
            ITurnContext turnContext,
            CancellationToken cancellationToken);

        Task CreateChannelUserIfNotExists(string channelId, string userId, string username);
        Task ValidateMagicNumber(ITurnContext turnContext, string text, CancellationToken cancellationToken);
    }
}