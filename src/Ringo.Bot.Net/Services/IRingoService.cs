using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using RingoBotNet.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public interface IRingoService
    {
        Task<Station> CreateChannelStation(
            string channelUserId,
            ConversationInfo info,
            Models.Album album = null,
            Models.Artist artist = null,
            Models.Playlist playlist = null);

        Task<Station> CreateUserStation(
            string channelUserId,
            ConversationInfo info,
            Models.Album album = null,
            Models.Artist artist = null,
            Models.Playlist playlist = null);

        Task<Playlist[]> FindPlaylists(string searchText, string accessToken, CancellationToken cancellationToken);

        Task PlayPlaylist(string playlistId, string accessToken, CancellationToken cancellationToken);

        Task<bool> JoinPlaylist(
            string query,
            string token,
            Station station,
            string stationToken,
            CancellationToken cancellationToken);

        Task<ChannelUser> CreateChannelUserIfNotExists(string channelId, string userId, string username);

        Task<SpotifyApi.NetCore.Device[]> GetDevices(string accessToken);

        Task<SpotifyApi.NetCore.CurrentPlaybackContext> GetUserNowPlaying(string token);

        Task<Models.Album> GetAlbum(string token, string uri);

        Task<Models.Artist> GetArtist(string token, string uri);

        Task<Playlist> GetPlaylist(string token, string uri);

        Task<string> GetPlaylistTrackOneUrl(string token, Playlist playlist);

        Task<Station> GetChannelStation(ConversationInfo info, string conversationName = null);

        Task<Station> GetUserStation(ConversationInfo info, string username);

    }
}