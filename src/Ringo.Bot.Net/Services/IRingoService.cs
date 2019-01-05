using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace Ringo.Bot.Net.Services
{
    public interface IRingoService
    {
        Task PlayPlaylist(ITurnContext turnContext, string searchText, string accessToken, CancellationToken cancellationToken);
    }
}