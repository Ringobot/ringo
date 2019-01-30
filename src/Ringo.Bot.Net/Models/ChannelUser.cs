using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using RingoBotNet.Helpers;
using SpotifyApi.NetCore;
using System;

namespace RingoBotNet.Models
{
    public class ChannelUser : CosmosDocument
    {
        public ChannelUser() { }

        public ChannelUser(
            string channelId, 
            string userId, 
            string username, 
            BearerAccessRefreshToken bearerAccessRefreshToken = null)
        {
            Id = EncodeId(channelId, userId);
            PartitionKey = new PartitionKey(Id);
            UserId = userId;
            Username = username;
            ChannelId = channelId;
            BearerAccessRefreshToken = bearerAccessRefreshToken;
        }

        /// <summary>
        /// Channel specific username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Channel specific unique id for the User.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// A name to identify the Bot Channel that this user belongs to (Slack, Skype, Teams, ...)
        /// </summary>
        public string ChannelId { get; set; }

        public BearerAccessRefreshToken BearerAccessRefreshToken { get; set; }

        public static string EncodeId(string channelId, string userId)
        {
            string uid = $"{channelId.Trim()}:{userId.Trim()}";
            return CryptoHelper.Base64Encode(uid.ToLower());
        }

        public override void EnforceInvariants()
        {
            base.EnforceInvariants();
            if (string.IsNullOrEmpty(UserId)) throw new InvariantException("UserId must not be null or empty");
            if (string.IsNullOrEmpty(ChannelId)) throw new InvariantException("ChannelId must not be null or empty");
            if (Id != EncodeId(ChannelId, UserId))
                throw new InvariantException("Id must be a hash of ChannelId and UserId. See `EncodeId`.");
        }
    }
}
