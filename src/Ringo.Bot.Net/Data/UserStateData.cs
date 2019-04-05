using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using System;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public class StateData : IStateData
    {
        private readonly CloudTable _table;
        private readonly ILogger _logger;

        public StateData(IConfiguration configuration, ILogger<StateData> logger)
        {
            var account = CloudStorageAccount.Parse(configuration[ConfigHelper.StorageConnectionString]);
            var client = account.CreateCloudTableClient();
            _table = client.GetTableReference(configuration[ConfigHelper.StateTableName]);
            _table.CreateIfNotExists();
        }

        public async Task<string> GetUserIdFromStateToken(string state)
        {
            if (string.IsNullOrEmpty(state)) throw new ArgumentNullException(nameof(state));

            TableOperation operation = TableOperation.Retrieve<UserState>(state, state);
            var result = await _table.ExecuteAsync(operation);
            var userState = result.Result as UserState;

            // if no document, or state token is stale, return null
            if (userState == null || userState.CreatedDate < DateTime.UtcNow.AddMinutes(-30)) return null;

            // returns the entity Id
            return userState.UserId;
        }

        /// <summary>
        /// Save a User State Token for a ChannelUser ID
        /// </summary>
        /// <param name="userId">The Id of the ChannelUser document (not the UserId)</param>
        /// <param name="state">The State value</param>
        /// <returns>The created UserState document</returns>
        public async Task SaveStateToken(string userId, string state)
        {
            var userState = new UserState(userId, state);
            var operation = TableOperation.Insert(userState);
            await _table.ExecuteAsync(operation);
        }
    }
}
