using System.Threading.Tasks;
using SpotifyApi.NetCore;

namespace RingoBotNet.Data
{
    public interface IChannelUserData
    {
        Task SaveUserAccessToken(string channelUserId, BearerAccessRefreshToken token);

        Task<BearerAccessRefreshToken> GetUserAccessToken(string channelId, string userId);
    }
}