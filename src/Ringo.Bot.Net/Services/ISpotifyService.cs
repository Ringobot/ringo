using RingoBotNet.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public interface ISpotifyService
    {
        Task<Playlist[]> FindPlaylists(string searchText, string accessToken, CancellationToken cancellationToken);

        Task PlayPlaylist(string playlistId, string accessToken, CancellationToken cancellationToken);

        Task<bool> JoinPlaylist(
            string query,
            string token,
            Station station,
            string stationToken,
            CancellationToken cancellationToken);

        Task<SpotifyApi.NetCore.Device[]> GetDevices(string accessToken);

        Task<SpotifyApi.NetCore.CurrentPlaybackContext> GetUserNowPlaying(string token);

        Task<Album> GetAlbum(string token, string uri);

        Task<Artist> GetArtist(string token, string uri);

        Task<Playlist> GetPlaylist(string token, string uri);

        Task<string> GetPlaylistTrackOneUrl(string token, Playlist playlist);

        Task TurnOffShuffleRepeat(string token, SpotifyApi.NetCore.CurrentPlaybackContext info);
    }
}