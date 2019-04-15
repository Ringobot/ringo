using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public interface IRingoService
    {
        Task<Station> CreateConversationStation(
            ConversationInfo info,
            Album album = null,
            Playlist playlist = null);

        Task<Station> CreateUserStation(
            ConversationInfo info,
            Album album = null,
            Playlist playlist = null);

        Task<User> CreateUserIfNotExists(ConversationInfo info, string userId = null, string username = null);

        Task<Station> GetChannelStation(ConversationInfo info, string conversationName = null);

        Task<Station> GetUserStation(ConversationInfo info, string username);
    }
}