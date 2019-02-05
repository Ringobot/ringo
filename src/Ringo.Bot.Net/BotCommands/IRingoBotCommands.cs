using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using RingoBotNet.State;

namespace RingoBotNet
{
    public interface IRingoBotCommands
    {
        Task Join(ITurnContext turnContext, UserProfile userProfile, string query, CancellationToken cancellationToken);
    }
}