using Newtonsoft.Json;
using RingoBotNet.Helpers;
using System;

namespace RingoBotNet.Models
{
    public class Station
    {
        public Station() { }

        public Station(string channelUserId, Album album, Playlist playlist, string hashtag = null)
        {
            string name = album?.Name ?? playlist?.Name;

            Id = Guid.NewGuid().ToString("N");
            Name = name;
            ChannelUserId = channelUserId;
            Hashtag = hashtag ?? RingoBotHelper.NonWordRegex.Replace(name, string.Empty);
            Album = album;
            Playlist = playlist;
            CreatedDate = ModifiedDate = DateTime.UtcNow;
            IsActive = true;
            ListenerCount = 1;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string ChannelUserId { get; set; }

        public string Hashtag { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Album Album { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Playlist Playlist { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public int ListenerCount { get; set; }

        public void EnforceInvariants()
        {
            if (string.IsNullOrEmpty(ChannelUserId)) throw new InvariantNullException(nameof(ChannelUserId));
            if (Album == null && Playlist == null) throw new InvariantException("Station must have Album or Playlist property set.");
            if (Album != null && Playlist != null) 
            {
                throw new InvariantException("Station must have only one of Album or Playlist property set.");
            }
        }

        [JsonIgnore]
        public string SpotifyContextType {
            get
            {
                if (Album != null) return "album";
                if (Playlist != null) return "playlist";
                return null;
            }
        }

        [JsonIgnore]
        public string SpotifyUri => Album?.Uri ?? Playlist?.Uri;
    }
}
