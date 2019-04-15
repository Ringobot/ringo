using Newtonsoft.Json;
using System;

namespace RingoBotNet.Models
{
    public class Listener : CosmosEntity
    {
        private const string TypeName = "Listener";

        public Listener() { }

        public Listener(Station station, User user)
        {
            Id = user.Id;
            PartitionKey = station.PartitionKey;
            Type = TypeName;

            User = user;
            Station = station;
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// User.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Station.
        /// </summary>
        public Station Station { get; set; }

        /// <summary>
        /// The date this entity was first created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Optional. The approximate date + time this Listener was last active.
        /// </summary>
        [JsonIgnore]
        public DateTime? LastActiveDate { get; set; }

        internal override void EnforceInvariants(bool isRoot = false)
        {
            base.EnforceInvariants();
            if (User == null) throw new InvariantNullException(nameof(User));
            if (Station == null) throw new InvariantNullException(nameof(Station));

            if (isRoot)
            {
                User.EnforceInvariants();
                Station.EnforceInvariants();
            }
        }
    }
}
