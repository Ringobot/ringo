using SpotifyApi.NetCore;
using System;

namespace RingoBotNet.Models
{
    public class Station //: CosmosDocument
    {
        public Station(PlaylistSimplified playlist)
        {
            //PartitionKey = Id = Hashcode = hashcode;
            Playlist = playlist;
            CreatedDate = DateTime.UtcNow;
            IsActive = true;
            ListenerCount = 1;
        }

        public string ChannelUserId { get; set; }

        public string ChannelId { get; set; }

        public string Username { get; set; }

        public string Hashcode { get; set; }

        public PlaylistSimplified Playlist { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        public int ListenerCount { get; set; }
    }
}
