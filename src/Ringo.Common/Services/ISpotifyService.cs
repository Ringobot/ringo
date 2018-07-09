using System.Threading.Tasks;

namespace Ringo.Common.Services
{
    public interface ISpotifyService
    {
        Task<dynamic> GetArtist(string artistId);
        Task<dynamic> GetPlaylists(string username);
        Task<dynamic> GetPlaylists(string username, int offset);
        Task<dynamic> GetRecommendation(string artistSeed, int limit = 3);
        Task<dynamic> GetRelatedArtists(string artistId);
        Task PlayArtist(string userHash, string spotifyUri);
        Task<dynamic> SearchArtists(string artistId, int limit = 3);
    }
}