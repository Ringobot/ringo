using RingoBotNet.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IChannelUserData
    {
        Task SaveUserAccessToken(string channelUserId, BearerAccessToken token);

        Task<BearerAccessToken> GetUserAccessToken(string channelId, string userId);

        Task<BearerAccessToken> GetUserAccessToken(string channelUserId);

        Task<ChannelUser> CreateChannelUserIfNotExists(string channelId, string userId, string username);

        Task SetTokenValidated(string channelId, string userId);

        Task<Station> CreateStation(
            string channelUserId,
            Album album = null,
            Artist artist = null,
            Playlist playlist = null,
            string hashtag = null);

        Task ResetAuthorization(string channelUserId, CancellationToken cancellationToken);

        Task<Station> GetStation(string channelUserId, string stationId);
    }
}