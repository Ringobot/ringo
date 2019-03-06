using RingoBotNet.Helpers;
using System;

namespace RingoBotNet.Models
{
    public class Station
    {
        public Station() { }

        public Station(string channelUserId, Album album, Artist artist, Playlist playlist, string hashtag = null)
        {
            string name = album?.Name ?? artist?.Name ?? playlist?.Name;

            Id = Guid.NewGuid().ToString("N");
            Name = name;
            ChannelUserId = channelUserId;
            Hashtag = hashtag ?? RingoBotHelper.NonWordRegex.Replace(name, string.Empty);
            Album = album;
            Artist = artist;
            Playlist = playlist;
            CreatedDate = ModifiedDate = DateTime.UtcNow;
            IsActive = true;
            ListenerCount = 1;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string ChannelUserId { get; set; }

        public string Hashtag { get; set; }

        public Album Album { get; set; }

        public Artist Artist { get; set; }

        public Playlist Playlist { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public int ListenerCount { get; set; }

        public void EnforceInvariants()
        {
            if (string.IsNullOrEmpty(ChannelUserId)) throw new InvariantNullException(nameof(ChannelUserId));
            if (Album == null && Artist == null && Playlist == null) throw new InvariantException("Station must have Album or Artist or Playlist property set.");
            if ((Album != null && (Artist != null || Playlist != null)) 
                || (Artist != null && (Album != null || Playlist != null))
                || (Playlist != null && (Album != null || Artist != null)))
            {
                throw new InvariantException("Station must have only one of Album or Artist or Playlist property set.");
            }
        }
    }
}
