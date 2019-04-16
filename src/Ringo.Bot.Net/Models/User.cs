using Newtonsoft.Json;
using RingoBotNet.Helpers;
using System;

namespace RingoBotNet.Models
{
    /// <summary>
    /// A User entity.
    /// </summary>
    public partial class User : CosmosEntity
    {
        private const string TypeName = "User";

        public User() { }

        public User(
            ConversationInfo info,
            string userId = null, 
            string username = null,
            BearerAccessToken spotifyAccessToken = null)
        {
            var now = DateTime.UtcNow;
            (string id, string pk) = EncodeIds(info, userId ?? info.FromId);

            Id = id;
            PartitionKey = pk;
            Type = TypeName;

            UserId = userId ?? info.FromId;
            Username = username ?? info.FromName;
            ChannelId = info.ChannelId;
            ChannelTeamId = info.ChannelTeamId;
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

        public string ChannelTeamId { get; set; }

        /// <summary>
        /// Authentication/Authorization details for Spotify service.
        /// </summary>
        public Auth SpotifyAuth { get; set; }

        /// <summary>
        /// Date that this Entity was first created.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }
    }

    /// <summary>
    /// Authorization / Authentication details for a service.
    /// </summary>
    public class Auth
    {
        public Auth()
        {

        }

        public Auth(BearerAccessToken accessToken)
        {
            BearerAccessToken = accessToken;
            CreatedDate = DateTime.UtcNow;
        }

        public Auth(string state)
        {
            State = state;
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Bearer access Token details
        /// </summary>
        public BearerAccessToken BearerAccessToken { get; set; }

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
    public class BearerAccessToken
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
