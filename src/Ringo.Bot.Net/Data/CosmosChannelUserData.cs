using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public class CosmosChannelUserData : CosmosData, IChannelUserData
    {
        public CosmosChannelUserData(
            IConfiguration configuration,
            ILogger<CosmosChannelUserData> logger,
            IDocumentClient documentClient,
            string databaseName,
            string collectionName) 
            : base(configuration, logger, documentClient, databaseName, collectionName)
        { }

        public async Task<BearerAccessToken> GetUserAccessToken(string channelId, string userId)
            => (await GetChannelUser(channelId, userId))?.SpotifyAccessToken;

        public async Task<BearerAccessToken> GetUserAccessToken(string channelUserId)
            => (await GetChannelUser(channelUserId))?.SpotifyAccessToken;

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

        public async Task<ChannelUser> CreateChannelUserIfNotExists(string channelId, string userId, string username)
        {
            var channelUser = await GetChannelUser(channelId, userId);
            if (channelUser == null) return await Create(new ChannelUser(channelId, userId, username));
            return channelUser;
        }

        public async Task SetTokenValidated(string channelId, string userId)
        {
            // looking forward to Patch!
            ChannelUser channelUser = await GetChannelUser(channelId, userId);
            if (channelUser.SpotifyAccessToken == null) throw new InvalidOperationException("Cannot validate a null SpotifyAccessToken");
            channelUser.SpotifyAccessToken.Validated = true;
            await Replace(channelUser);
        }

        public async Task<Station> CreateStation(
            string channelUserId, 
            Album album = null, 
            Artist artist = null, 
            Playlist playlist = null, 
            string hashtag = null)
        {
            var station = new Station(channelUserId, album, artist, playlist, hashtag);
            var channelUser = await GetChannelUser(channelUserId);

            if (channelUser.Stations == null) channelUser.Stations = new[] { station };
            else channelUser.Stations = channelUser.Stations.Concat(new[] { station });

            await Replace(channelUser);

            _logger.LogInformation($"Added Station Id = \"{station.Id} to Channel User Id = \"{channelUserId}\"");

            return station;
        }

        /// <summary>
        /// Resets Authorization by setting the SpotifyAccessToken to null.
        /// </summary>
        public async Task ResetAuthorization(string channelUserId, CancellationToken cancellationToken)
        {
            // looking forward to Patch!
            ChannelUser channelUser = await GetChannelUser(channelUserId);
            if (channelUser.SpotifyAccessToken == null) return;
            channelUser.SpotifyAccessToken = null;
            await Replace(channelUser);
            _logger.LogInformation($"Reset channelUser.SpotifyAccessToken = null for channelUserId = \"{channelUserId}\"");
        }

        public async Task<Station> GetStation(string channelUserId, string stationId)
        {
            var channelUser = await GetChannelUser(channelUserId);
            return channelUser?.Stations.FirstOrDefault(s => s.Id == stationId);
        }
    }
}
