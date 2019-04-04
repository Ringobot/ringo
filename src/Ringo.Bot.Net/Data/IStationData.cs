using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IStationData
    {
        //Task CreateStationUri(string stationId, string channelUserId, string uri, string hashtag = null);

        //Task<StationUri> GetStationUri(string uri);

        Task<Station2> CreateStation(
            string stationUri,
            User owner,
            Album album = null,
            Playlist playlist = null,
            string hashtag = null);

        Task<Station2> GetStation(string stationUri);

    }
}
