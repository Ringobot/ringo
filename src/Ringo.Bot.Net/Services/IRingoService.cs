using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public interface IRingoService
    {
        Task<Station> CreateChannelStation(
            string userId,
            ConversationInfo info,
            Album album = null,
            Playlist playlist = null);

        Task<Station> CreateUserStation(
            string userId,
            ConversationInfo info,
            Album album = null,
            Playlist playlist = null);

        Task<User> CreateUserIfNotExists(string channelId, string userId, string username);

        Task<Station> GetChannelStation(ConversationInfo info, string conversationName = null);

        Task<Station> GetUserStation(ConversationInfo info, string username);
    }
}