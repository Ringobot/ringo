using RingoBotNet.Helpers;
using System;

namespace RingoBotNet.Models
{
    public class StationUri : TableEntity
    {
        public StationUri() { }

        public StationUri(string uri, string channelUserId, string stationId, string hashtag = null)
        {
            if (string.IsNullOrEmpty(uri)) throw new ArgumentNullException(nameof(uri));
            if (string.IsNullOrEmpty(channelUserId)) throw new ArgumentNullException(nameof(channelUserId));
            if (string.IsNullOrEmpty(stationId)) throw new ArgumentNullException(nameof(stationId));

            var keys = RowKeyPartitionKey(uri);
            RowKey = keys.RowKey;
            PartitionKey = keys.PartitionKey;
            Uri = uri;
            ChannelUserId = channelUserId;
            StationId = stationId;
            Hashtag = hashtag;
        }

        public string ChannelUserId { get; set; }

        public string Uri { get; set; }

        public string StationId { get; set; }

        public string Hashtag { get; set; }

        public override void EnforceInvariants()
        {
            base.EnforceInvariants();
            if (string.IsNullOrEmpty(ChannelUserId)) throw new InvariantNullException(nameof(ChannelUserId));
            if (string.IsNullOrEmpty(Uri)) throw new InvariantNullException(nameof(Uri));
            if (string.IsNullOrEmpty(StationId)) throw new InvariantNullException(nameof(StationId));
        }

        public static (string RowKey, string PartitionKey) RowKeyPartitionKey(string uri)
        {
            string key = CryptoHelper.Sha256(uri);
            return (key, key);
        }
    }
}
