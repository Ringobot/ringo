using RingoBotNet.Helpers;
using System;

namespace RingoBotNet.Models
{
    public class StationUri : CosmosDocument
    {
        public StationUri(string uri, string channelUserId, string stationId, string hashtag = null)
        {
            Id = CryptoHelper.Base64Encode(uri);
            PartitionKey = CryptoHelper.Sha256(uri);
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
    }
}
