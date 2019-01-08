using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Ringo.Bot.Net.State;

namespace Ringo.Bot.Net.Services
{
    public interface IRingoService
    {
        Task PlayPlaylist(ITurnContext turnContext, string searchText, string accessToken, CancellationToken cancellationToken);

        Task JoinPlaylist(ITurnContext turnContext, string joinUsername, ConversationData conversationData, string token, CancellationToken cancellationToken);
    }
}