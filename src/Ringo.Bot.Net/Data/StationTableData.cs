using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RingoBotNet.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using RingoBotNet.Helpers;

namespace RingoBotNet.Data
{
    public class StationTableData : IStationData
    {
        private readonly CloudTable _table;

        public StationTableData(IConfiguration configuration)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(configuration[ConfigHelper.StorageConnectionString]);
            CloudTableClient client = account.CreateCloudTableClient();
            _table = client.GetTableReference(configuration[ConfigHelper.StationUriTableName]);
            _table.CreateIfNotExists();
        }

        public async Task<StationUri> GetStationUri(string uri)
        {
            if (string.IsNullOrEmpty(uri)) throw new ArgumentNullException(nameof(uri));

            var (RowKey, PartitionKey) = StationUri.RowKeyPartitionKey(uri);
            TableOperation operation = TableOperation.Retrieve<StationUri>(PartitionKey, RowKey);
            var result = await _table.ExecuteAsync(operation);
            return result.Result as StationUri;
        }

        public async Task CreateStationUri(string stationId, string channelUserId, string uri, string hashtag = null)
        {
            var stationUri = new StationUri(uri, channelUserId, stationId, hashtag);
            var operation = TableOperation.InsertOrReplace(stationUri);
            await _table.ExecuteAsync(operation);
        }
    }
}
