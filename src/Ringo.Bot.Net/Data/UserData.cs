//using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using System;
using System.Security;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public class UserData : CosmosData, IUserData
    {
        public UserData(
            IConfiguration configuration,
            ILogger<UserData> logger)
            : base(configuration, logger, configuration[ConfigHelper.CosmosDBUserCollectionName])
        { }

        public async Task<User> GetUser(string userId) => await Read<User>(userId);

        public async Task SaveUserAccessToken(string channelUserId, BearerAccessToken token)
        {
            var user = await GetUser(channelUserId);
            if (user == null) throw new InvalidOperationException($"User not found. Id = {channelUserId}");
            if (user.SpotifyAuth == null) user.SpotifyAuth = new Auth(token);
            else user.SpotifyAuth.BearerAccessToken = token;

            // optimistic concurrency... :\
            await Replace(user);
        }

        public async Task<User> CreateUserIfNotExists(string channelId, string userId, string username)
        {
            string channelUserId = User.EncodeId(channelId, userId);
            var user = await GetUser(channelUserId);

            if (user == null)
            {
                user = new User(channelId, userId, username);
                await Create(user);
            }

            return user;
        }

        public async Task SetTokenValidated(string channelUserId, string state)
        {
            // looking forward to Patch!
            var user = await GetUser(channelUserId);
            if (user.SpotifyAuth == null || user.SpotifyAuth.State != state || user.SpotifyAuth.BearerAccessToken == null)
            {
                throw new SecurityException("Invalid State Token");
            }

            user.SpotifyAuth.ValidatedDate = DateTime.UtcNow;
            await Replace(user);
        }

        /// <summary>
        /// Resets Authorization by setting the SpotifyAccessToken to null.
        /// </summary>
        public async Task ResetAuthorization(string channelUserId)
        {
            // looking forward to Patch!
            var user = await GetUser(channelUserId);
            if (user.SpotifyAuth == null) return;
            user.SpotifyAuth = null;
            await Replace(user);
            _logger.LogInformation($"Reset User.SpotifyAuth = null for User.Id = \"{channelUserId}\"");
        }

        
        public async Task SaveStateToken(string userId, string state)
        {
            // looking forward to Patch!
            var user = await GetUser(userId);
            user.SpotifyAuth = new Auth(state);
            await Replace(user);

            _logger.LogDebug($"Reset User.SpotifyAuth, state = {state} for User.Id = \"{userId}\"");
        }
    }
}
