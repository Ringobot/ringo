using Newtonsoft.Json;
using System;

namespace RingoBotNet.Models
{
    public class UserState : CosmosDocument
    {
        public UserState() { }

        public UserState(string channelUserId, string state)
        {
            ChannelUserId = channelUserId;
            Id = State = state;
            PartitionKey = Id;
            CreatedDate = DateTime.UtcNow;
        }

        public UserState(string channelId, string userId, string state)
        {
            ChannelUserId = ChannelUser.EncodeId(channelId, userId);
            Id = State = state;
            PartitionKey = Id;
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// The document Id of the ChannelUser (not the UserId)
        /// </summary>
        public string ChannelUserId { get; set; }

        public string State { get; set; }

        public DateTime CreatedDate { get; set; }

        ///// <summary>
        ///// Time to live (TTL) in seconds. Used to set expiration policy 
        ///// </summary>
        ///// <remarks> https://docs.microsoft.com/en-us/azure/cosmos-db/how-to-time-to-live#set-time-to-live-on-an-item </remarks>
        //[JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        //public int? TimeToLive { get; set; }

        public override void EnforceInvariants()
        {
            base.EnforceInvariants();
            if (State != Id) throw new InvariantException("Id must equal State");
        }
    }
}
