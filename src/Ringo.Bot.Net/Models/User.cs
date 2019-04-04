using Newtonsoft.Json;
using RingoBotNet.Helpers;
using System;

namespace RingoBotNet.Models
{
    /// <summary>
    /// A User entity.
    /// </summary>
    public class User : CosmosEntity
    {
        private const string TypeName = "User";

        public User() { }

        public User(
            string channelId,
            string userId,
            string username,
            BearerAccessToken2 spotifyAccessToken = null)
        {
            var now = DateTime.UtcNow;

            Id = EncodeId(channelId, userId);
            PartitionKey = Id;
            Type = TypeName;
            UserId = userId;
            Username = username;
            ChannelId = channelId;
            CreatedDate = now;


            if (spotifyAccessToken != null)
            {
                SpotifyAuth = new Auth { BearerAccessToken = spotifyAccessToken, CreatedDate = now };
            }
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

        /// <summary>
        /// Authentication/Authorization details for Spotify service.
        /// </summary>
        public Auth SpotifyAuth { get; set; }

        /// <summary>
        /// Date that this Entity was first created.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Encode a User.Id from a ChannelId and UserId provided by the bot channel. This is a one-way hash.
        /// </summary>
        /// <param name="channelId">ChannelID for the bot services, e.g. "msteams". Not case sensitive.</param>
        /// <param name="userId">UserId as provided by the bot service. Not case sensitive.</param>
        /// <returns></returns>
        public static string EncodeId(string channelId, string userId)
        {
            string uid = $"{channelId.Trim()}:{userId.Trim()}";
            return CryptoHelper.Sha256(uid.ToLower());
        }

        public override void EnforceInvariants(bool isRoot = false)
        {
            base.EnforceInvariants();
            if (Type != TypeName) throw new InvariantException("Type must not be null or empty");
            if (string.IsNullOrEmpty(UserId)) throw new InvariantException("UserId must not be null or empty");
            if (string.IsNullOrEmpty(ChannelId)) throw new InvariantException("ChannelId must not be null or empty");
            if (Id != EncodeId(ChannelId, UserId))
                throw new InvariantException("Id must be a hash of ChannelId and UserId. See `EncodeId`.");
        }
    }

    /// <summary>
    /// Authorization / Authentication details for a service.
    /// </summary>
    public class Auth
    {
        public Auth()
        {

        }

        public Auth(BearerAccessToken2 accessToken)
        {
            BearerAccessToken = accessToken;
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Bearer access Token details
        /// </summary>
        public BearerAccessToken2 BearerAccessToken { get; set; }

        /// <summary>
        /// Date this entity was first created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// State token used during Authentication workflow
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Date and time that these details were validated by the user (by magic number for example).
        /// </summary>
        public DateTime? ValidatedDate { get; set; }

        /// <summary>
        /// True when a User has validated the Token using a Magic Number (for example).
        /// </summary>
        [JsonIgnore]
        public bool Validated { get => ValidatedDate.HasValue; }
    }

    /// <summary>
    /// Bearer access Token details
    /// </summary>
    public class BearerAccessToken2
    {
        /// <summary>
        /// The Refresh token for this user/service.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// An access (bearer) token for this user/service.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The scope of the token.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The date/time that the token expires.
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// True when the Access Token has expired
        /// </summary>
        [JsonIgnore]
        public bool AccessTokenExpired { get => Expires.HasValue && Expires.Value < DateTime.UtcNow; }
    }
}
