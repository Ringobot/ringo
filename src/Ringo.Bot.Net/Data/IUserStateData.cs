using System.Threading.Tasks;
using RingoBotNet.Models;
using SpotifyApi.NetCore;

namespace RingoBotNet.Data
{
    public interface IStateData
    {
        Task<string> GetUserIdFromStateToken(string state);

        Task SaveStateToken(string channelUserId, string state);
    }
}