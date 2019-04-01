using Newtonsoft.Json;

namespace RingoBotNet.Models
{
    public abstract class CosmosEntity
    {
        /// <summary>
        /// Unique Entity Id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The Partition Key for this Entity.
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// The type of this Entity.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// When called, checks that the state of the entity is valid, based on a set of rules called invariants.
        /// </summary>
        public virtual void EnforceInvariants()
        {
            if (string.IsNullOrEmpty(Id)) throw new InvariantNullException(nameof(Id));
            if (string.IsNullOrEmpty(PartitionKey)) throw new InvariantNullException(nameof(PartitionKey));
            if (string.IsNullOrEmpty(Type)) throw new InvariantNullException(nameof(Type));
        }
    }
}