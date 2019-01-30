using System.Threading.Tasks;
using RingoBotNet.Models;
using SpotifyApi.NetCore;

namespace RingoBotNet.Data
{
    public interface IUserStateData
    {
        Task<string> GetChannelUserIdFromStateToken(string state);

        Task<UserState> SaveUserStateToken(string channelUserId, string state);

        Task<UserState> SaveUserStateToken(string channelId, string userId, string state);
    }
}