using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using System;
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

            _logger.LogDebug($"CreateStation: {station}");

            return station;
        }

        public async Task ReplaceStation(string stationUri, Station station)
        {
            if (station == null) throw new ArgumentNullException(nameof(station));
            if (stationUri != station.Uri)
                throw new InvalidOperationException($"stationUri \"{stationUri}\" is not the same as Station.Uri \"{station.Uri}\".");
            await Replace(station);
            _logger.LogDebug($"UpdateStation: {station}");
        }

        public async Task<Station> GetStation(string stationUri) => await Read<Station>(Station.EncodeIds(stationUri));
    }
}
