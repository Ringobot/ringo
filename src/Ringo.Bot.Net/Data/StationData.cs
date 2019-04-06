using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public class StationData : CosmosData, IStationData
    {
        public StationData(
            IConfiguration configuration,
            ILogger<StationData> logger)
            : base(configuration, logger, configuration[ConfigHelper.CosmosDBStationCollectionName])
        { }

        public async Task<Station> CreateStation(
            string stationUri,
            User owner,
            Album album = null,
            Playlist playlist = null,
            string hashtag = null)
        {
            var station = new Station(stationUri, album, playlist, owner, hashtag);
            await Create(station);

            _logger.LogInformation($"Created Station, Uri = \"{station.Uri}");

            return station;
        }

        public async Task<Station> GetStation(string stationUri)
        {
            return await Read<Station>(stationUri);
        }
    }
}
