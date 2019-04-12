using Newtonsoft.Json;
using RingoBotNet.Helpers;
using System;
using System.Linq;

namespace RingoBotNet.Models
{
    public class Station : CosmosEntity
    {
        private const string TypeName = "Station";

        public Station() { }

        public Station(string uri, Album album, Playlist playlist, User owner, string hashtag = null)
        {
            if (string.IsNullOrEmpty(uri)) throw new ArgumentNullException(uri);

            (string id, string pk) = EncodeIds(uri);
            Id = id;
            PartitionKey = pk;
            Type = TypeName;

            string name = album?.Name ?? playlist?.Name;

            Uri = uri;
            Uid = Guid.NewGuid().ToString("N");
            Name = name;
            Hashtag = hashtag ?? RingoBotHelper.ToHashtag(name);
            Album = album;
            Playlist = playlist;
            Owner = owner;
            CreatedDate = DateTime.UtcNow;
            IsActive = true;
            ListenerCount = 1;

            ActiveListeners = new[] { new Listener(this, owner) };
        }

        /// <summary>
        /// A GUID for this Station.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// A globally unique resource id for this Station.
        /// </summary>
        public string Uri { get; set; }

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

        /// <summary>
        /// Encodes the Id and Partition Key into a format suitable for a <see cref="CosmosEntity"/>
        /// </summary>
        /// <param name="stationUri">The Station URI</param>
        public static (string id, string pk) EncodeIds(string stationUri)
        {
            string id = EncodeId(stationUri);
            return (id, id);
        }

        public override void EnforceInvariants(bool isRoot = false)
        {
            base.EnforceInvariants();
            if (string.IsNullOrEmpty(Uri)) throw new InvariantException("Uri must not be null");
            if (string.IsNullOrEmpty(Hashtag)) throw new InvariantException("Hashtag must not be null");
            if (string.IsNullOrEmpty(Name)) throw new InvariantException("Name must not be null");
            if (Album == null && Playlist == null) throw new InvariantException("Station must have Album or Playlist property set.");

            if (Album != null && Playlist != null)
            {
                throw new InvariantException("Station must have only one of Album or Playlist property set.");
            }

            if (ListenerCount < 0) throw new InvariantException("ListenerCount must not be less than Zero");

            if (isRoot)
            {
                Owner.EnforceInvariants();
                foreach (var listener in ActiveListeners) listener.EnforceInvariants();
            }
        }
    }
}
