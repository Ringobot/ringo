using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace RingoBotNet.Models
{
    public abstract class CosmosDocument
    {
        /// <summary>
        /// Globally unique Id for Storage. Internal use only.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string PartitionKey { get; set; }

        public virtual void EnforceInvariants()
        {
            if (string.IsNullOrEmpty(Id)) throw new InvariantNullException(nameof(Id));
            if (PartitionKey == null) throw new InvariantNullException(nameof(PartitionKey));
        }
    }
}