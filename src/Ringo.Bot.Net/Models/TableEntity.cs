namespace RingoBotNet.Models
{
    public abstract class TableEntity : Microsoft.Azure.Cosmos.Table.TableEntity
    {
        public virtual void EnforceInvariants()
        {
            if (string.IsNullOrEmpty(RowKey)) throw new InvariantNullException(nameof(RowKey));
            if (PartitionKey == null) throw new InvariantNullException(nameof(PartitionKey));
        }
    }
}
