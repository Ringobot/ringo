using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using RingoBotNet.Models;
using SpotifyApi.NetCore;
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

        public async Task<BearerAccessRefreshToken> GetUserAccessToken(string channelId, string userId)
            => (await GetChannelUser(channelId, userId))?.BearerAccessRefreshToken;

        private async Task<ChannelUser> GetChannelUser(string channelId, string userId)
        {
            string id = ChannelUser.EncodeId(channelId, userId);
            return await Read<ChannelUser>(id, id);
        }

        private async Task<ChannelUser> GetChannelUser(string channelUserId)
        {
            return await Read<ChannelUser>(channelUserId);
        }

        public async Task SaveUserAccessToken(string channelUserId, BearerAccessRefreshToken token)
        {
            ChannelUser channelUser = await GetChannelUser(channelUserId);
            if (channelUser == null) throw new InvalidOperationException($"ChannelUser not found. Id = {channelUserId}");
            channelUser.BearerAccessRefreshToken = token;
            // optimistic concurrency... :\
            await Replace(channelUser);
        }
    }
}
