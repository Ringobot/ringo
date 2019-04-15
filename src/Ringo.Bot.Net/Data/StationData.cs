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

        public async Task CreateStation(Station station)
        {
            await Create(station);
        }

        public async Task ReplaceStation(Station station)
        {
            if (station == null) throw new ArgumentNullException(nameof(station));
            await Replace(station);
        }

        public async Task<Station> GetStation((string id, string pk) stationIds) => await Read<Station>(stationIds);
    }
}
