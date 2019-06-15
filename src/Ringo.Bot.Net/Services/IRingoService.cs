using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public interface IRingoService
    {
        /// <summary>
        /// Changes the station owner to the current conversation From user
        /// </summary>
        Task ChangeStationOwner(Station station, ConversationInfo info);

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