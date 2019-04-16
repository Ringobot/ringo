using Newtonsoft.Json;
using System;

namespace RingoBotNet.Models
{
    //TODO public class UserStation : Station
    //TODO public class ConversationStation : Station

    public partial class Station : CosmosEntity
    {
        private const string TypeName = "Station";

        public Station() { }

        public Station(ConversationInfo info, string hashtag, Album album, Playlist playlist, User owner, string username = null)
        {
            (string id, string pk) = EncodeIds(info, hashtag, username);
            Id = id;
            PartitionKey = pk;
            Type = TypeName;

            string name = album?.Name ?? playlist?.Name;

            Name = name;
            Hashtag = hashtag;
            Album = album;
            Playlist = playlist;
            Owner = owner;
            CreatedDate = DateTime.UtcNow;
            IsActive = true;
            IsUserStation = !string.IsNullOrEmpty(username);
            ListenerCount = 1;

            ActiveListeners = new[] { new Listener(this, owner) };
        }

        /// <summary>
        /// The name of this Station.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A hastag for this Station. Must not include "#".
        /// </summary>
        public string Hashtag { get; set; }

        /// <summary>
        /// The Album that this Station is currently playing.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Album Album { get; set; }

        /// <summary>
        /// The Playlist that this Station is currently playing.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Playlist Playlist { get; set; }

        public User Owner { get; set; }

        /// <summary>
        /// The date this entity was first created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// True when this station is Active. False when no one is listening to it.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// True when this is a User Station. False when this is a Conversation Station.
        /// </summary>
        public bool IsUserStation { get; set; }

        /// <summary>
        /// An estimate of the number of users that are currently listening to this station.
        /// </summary>
        public int ListenerCount { get; set; }

        /// <summary>
        /// Maximum 10 of the most recent active Listeners
        /// </summary>
        public Listener[] ActiveListeners { get; set; }

        /// <summary>
        /// Derives the Spotify Context type for the context that this Station is currently playing.
        /// </summary>
        [JsonIgnore]
        public string SpotifyContextType
        {
            get
            {
                if (Album != null) return "album";
                if (Playlist != null) return "playlist";
                return null;
            }
        }

        /// <summary>
        /// Derives the Spotify URI for the context that this Station is currently playing.
        /// </summary>
        [JsonIgnore]
        public string SpotifyUri => Album?.Uri ?? Playlist?.Uri;
    }
}
