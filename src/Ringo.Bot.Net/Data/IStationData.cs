using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IStationData
    {
        //Task CreateStationUri(string stationId, string channelUserId, string uri, string hashtag = null);

        //Task<StationUri> GetStationUri(string uri);

        Task<Station> CreateStation(
            string stationUri,
            User owner,
            Album album = null,
            Playlist playlist = null,
            string hashtag = null);

        Task ReplaceStation(string stationUri, Station station);

        Task<Station> GetStation(string stationUri);

    }
}
