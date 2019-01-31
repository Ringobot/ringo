using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using RingoBotNet.Models;
using System;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public class CosmosChannelUserData : CosmosData, IChannelUserData
    {
        public CosmosChannelUserData(
            IConfiguration configuration,
            IDocumentClient documentClient,
            string databaseName,
            string collectionName) : base(configuration, documentClient, databaseName, collectionName)
        { }

        public async Task<BearerAccessToken> GetUserAccessToken(string channelId, string userId)
            => (await GetChannelUser(channelId, userId))?.SpotifyAccessToken;

        private async Task<ChannelUser> GetChannelUser(string channelId, string userId)
        {
            string id = ChannelUser.EncodeId(channelId, userId);
            return await Read<ChannelUser>(id, id);
        }

        private async Task<ChannelUser> GetChannelUser(string channelUserId)
        {
            return await Read<ChannelUser>(channelUserId);
        }

        public async Task SaveUserAccessToken(string channelUserId, BearerAccessToken token)
        {
            ChannelUser channelUser = await GetChannelUser(channelUserId);
            if (channelUser == null) throw new InvalidOperationException($"ChannelUser not found. Id = {channelUserId}");
            channelUser.SpotifyAccessToken = token;
            // optimistic concurrency... :\
            await Replace(channelUser);
        }

        public async Task CreateChannelUserIfNotExists(string channelId, string userId, string username)
        {
            var channelUser = await GetChannelUser(channelId, userId);
            if (channelUser == null) await Create(new ChannelUser(channelId, userId, username));
        }

        public async Task SetTokenValidated(string channelId, string userId)
        {
            // looking forward to Patch!
            ChannelUser channelUser = await GetChannelUser(channelId, userId);
            if (channelUser.SpotifyAccessToken == null) throw new InvalidOperationException("Cannot validate a null SpotifyAccessToken");
            channelUser.SpotifyAccessToken.Validated = true;
            await Replace(channelUser);
        }
    }
}
