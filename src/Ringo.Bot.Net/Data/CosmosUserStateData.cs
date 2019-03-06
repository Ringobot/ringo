using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Models;
using System;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public class CosmosUserStateData : CosmosData, IUserStateData
    {
        public CosmosUserStateData(
            IConfiguration configuration,
            ILogger<CosmosUserStateData> logger,
            IDocumentClient documentClient,
            string databaseName,
            string collectionName)
            : base(configuration, logger, documentClient, databaseName, collectionName)
        { }

        internal async Task Init()
        {
            // Turn on TTL
            DocumentCollection collection = await _client.ReadDocumentCollectionAsync(_collectionUri);
            collection.DefaultTimeToLive = 60 * 30; // 30 mins
            await _client.ReplaceDocumentCollectionAsync(collection);
        }

        public async Task<string> GetChannelUserIdFromStateToken(string state)
        {
            UserState userState = await Read<UserState>(state);

            // if no document, or state token is stale, return null
            if (userState == null || userState.CreatedDate < DateTime.UtcNow.AddMinutes(-30)) return null;

            // returns the entity Id
            return userState.ChannelUserId;
        }

        /// <summary>
        /// Save a User State Token for a ChannelUser ID
        /// </summary>
        /// <param name="channelUserId">The Id of the ChannelUser document (not the UserId)</param>
        /// <param name="state">The State value</param>
        /// <returns>The created UserState document</returns>
        public async Task<UserState> SaveUserStateToken(string channelUserId, string state)
            => await Create(new UserState(channelUserId, state));

        /// <summary>
        /// Save a User State Token for a ChannelUser ID
        /// </summary>
        /// <param name="channelUserId">The Id of the ChannelUser document (not the UserId)</param>
        /// <param name="state">The State value</param>
        /// <returns>The created UserState document</returns>
        public async Task<UserState> SaveUserStateToken(string channelId, string userId, string state)
            => await Create(new UserState(channelId, userId, state));
    }
}
