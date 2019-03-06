using Newtonsoft.Json;
using RingoBotNet.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RingoBotNet.Models
{
    public class ChannelUser : CosmosDocument
    {
        public ChannelUser() { }

        public ChannelUser(
            string channelId,
            string userId,
            string username,
            BearerAccessToken spotifyAccessToken = null)
        {
            Id = EncodeId(channelId, userId);
            PartitionKey = Id;
            UserId = userId;
            Username = username;
            ChannelId = channelId;
            SpotifyAccessToken = spotifyAccessToken;
            CreatedDate = DateTime.UtcNow;
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

        public BearerAccessToken SpotifyAccessToken { get; set; }

        public IEnumerable<Station> Stations { get; set; }

        [JsonIgnore]
        public Station CurrentStation { get => Stations.FirstOrDefault(s => s.IsActive); }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }

        public static string EncodeId(string channelId, string userId)
        {
            string uid = $"{channelId.Trim()}:{userId.Trim()}";
            return CryptoHelper.Sha256(uid.ToLower());
        }

        public override void EnforceInvariants()
        {
            base.EnforceInvariants();
            if (string.IsNullOrEmpty(UserId)) throw new InvariantException("UserId must not be null or empty");
            if (string.IsNullOrEmpty(ChannelId)) throw new InvariantException("ChannelId must not be null or empty");
            if (Id != EncodeId(ChannelId, UserId))
                throw new InvariantException("Id must be a hash of ChannelId and UserId. See `EncodeId`.");

            if (Stations != null) foreach (var station in Stations) station.EnforceInvariants();
        }
    }

    public class BearerAccessToken
    {
        public string RefreshToken { get; set; }

        public string AccessToken { get; set; }

        public string Scope { get; set; }

        public DateTime? Expires { get; set; }

        /// <summary>
        /// True when a User has validated the Token using a Magic Number (for example).
        /// </summary>
        public bool Validated { get; set; }

        [JsonIgnore]
        public bool AccessTokenExpired { get => Expires.HasValue && Expires.Value < DateTime.UtcNow; }
    }
}
