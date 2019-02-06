using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using RingoBotNet.State;

namespace RingoBotNet
{
    public interface IRingoBotCommands
    {
        Task Auth(ITurnContext turnContext, UserProfile userProfile, string query, CancellationToken cancellationToken);

        Task Join(
            ITurnContext turnContext, 
            UserProfile userProfile, 
            string query, 
            CancellationToken cancellationToken, 
            TokenResponse token = null);

        Task Play(
            ITurnContext turnContext,
            UserProfile userProfile,
            string query,
            CancellationToken cancellationToken,
            TokenResponse token = null);
    }
}