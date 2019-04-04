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

        public async Task<Station2> CreateStation(
            string stationUri,
            User owner,
            Album album = null,
            Playlist playlist = null,
            string hashtag = null)
        {
            var station = new Station2(stationUri, album, playlist, owner, hashtag);
            await Create(station);

            _logger.LogInformation($"Created Station, Uri = \"{station.Uri}");

            return station;
        }

        public async Task<Station2> GetStation(string stationUri)
        {
            return await Read<Station2>(stationUri);
        }

        //public async Task<StationUri> GetStationUri(string uri)
        //{
        //    if (string.IsNullOrEmpty(uri)) throw new ArgumentNullException(nameof(uri));

        //    var (RowKey, PartitionKey) = StationUri.RowKeyPartitionKey(uri);
        //    TableOperation operation = TableOperation.Retrieve<StationUri>(PartitionKey, RowKey);
        //    var result = await _table.ExecuteAsync(operation);
        //    return result.Result as StationUri;
        //}

        //public async Task CreateStationUri(string stationId, string channelUserId, string uri, string hashtag = null)
        //{
        //    var stationUri = new StationUri(uri, channelUserId, stationId, hashtag);
        //    var operation = TableOperation.InsertOrReplace(stationUri);
        //    await _table.ExecuteAsync(operation);
        //}

    }
}
