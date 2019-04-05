using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public interface IRingoService
    {
        Task<Station2> CreateChannelStation(
            string userId,
            ConversationInfo info,
            Album album = null,
            Playlist playlist = null);

        Task<Station2> CreateUserStation(
            string userId,
            ConversationInfo info,
            Album album = null,
            Playlist playlist = null);

        Task<User> CreateUserIfNotExists(string channelId, string userId, string username);

        Task<Station2> GetChannelStation(ConversationInfo info, string conversationName = null);

        Task<Station2> GetUserStation(ConversationInfo info, string username);
    }
}