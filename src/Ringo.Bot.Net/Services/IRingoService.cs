using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using RingoBotNet.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public interface IRingoService
    {
        Task<Station> CreateStation(
            ITurnContext turnContext, 
            Playlist playlist, 
            CancellationToken cancellationToken, 
            string hashtag = null);

        Task<Playlist> PlayPlaylist(ITurnContext turnContext, string searchText, string accessToken, CancellationToken cancellationToken);

        Task JoinPlaylist(
            ITurnContext turnContext,
            string query,
            string token,
            Station station,
            string stationToken,
            CancellationToken cancellationToken);

        Task<ChannelUser> CreateChannelUserIfNotExists(string channelId, string userId, string username);

        Task<Station> FindStation(ITurnContext turnContext, string query, CancellationToken cancellationToken);

        Task<SpotifyApi.NetCore.CurrentPlaybackContext> GetUserNowPlaying(string token);

        Task<Playlist> GetPlaylist(string token, string uri);
    }
}