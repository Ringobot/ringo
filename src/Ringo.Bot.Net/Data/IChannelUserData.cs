using RingoBotNet.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IChannelUserData
    {
        Task SaveUserAccessToken(string channelUserId, BearerAccessToken token);

        Task<BearerAccessToken> GetUserAccessToken(string channelId, string userId);

        Task<ChannelUser> CreateChannelUserIfNotExists(string channelId, string userId, string username);

        Task SetTokenValidated(string channelId, string userId);

        Task<Station> CreateStation(
            string channelId,
            string userId,
            string username,
            string hashcode,
            SpotifyApi.NetCore.PlaylistSimplified playlistSimplified);

        Task ResetAuthorization(string channelUserId, CancellationToken cancellationToken);
    }
}