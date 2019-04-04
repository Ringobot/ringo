using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IUserData
    {
        Task SaveUserAccessToken(string channelUserId, BearerAccessToken2 token);

        Task<BearerAccessToken2> GetUserAccessToken(string channelUserId);

        Task<User> CreateUserIfNotExists(string channelId, string userId, string username);

        Task SetTokenValidated(string channelUserId, string state);

        Task ResetAuthorization(string channelUserId);
    }
}