using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IUserData
    {
        Task<User> CreateUserIfNotExists(string channelId, string userId, string username);

        Task<User> GetUser(string userId);

        Task ResetAuthorization(string channelUserId);

        Task SaveUserAccessToken(string channelUserId, BearerAccessToken2 token);

        Task SetTokenValidated(string channelUserId, string state);
    }
}