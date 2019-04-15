using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IUserData
    {
        Task<User> CreateUserIfNotExists(ConversationInfo info, string userId = null, string username = null);

        Task<User> GetUser(string userId);

        Task ResetAuthorization(string userId);

        Task SaveStateToken(string userId, string state);

        Task SaveUserAccessToken(string userId, BearerAccessToken token);

        Task SetTokenValidated(string userId, string state);
    }
}