using RingoBotNet.Helpers;
using System;

namespace RingoBotNet.Models
{
    public class Station
    {
        public Station(Playlist playlist, string hashtag = null)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = playlist.Name;
            Hashtag = hashtag ?? RingoBotHelper.NonWordRegex.Replace(playlist.Name, string.Empty);
            Playlist = playlist;
            CreatedDate = ModifiedDate = DateTime.UtcNow;
            IsActive = true;
            ListenerCount = 1;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Hashtag { get; set; }

        public Playlist Playlist { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public int ListenerCount { get; set; }
    }
}
