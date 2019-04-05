using System;

namespace RingoBotNet.Models
{
    /// <summary>
    /// Stores state values for Users
    /// </summary>
    public class UserState : TableEntity
    {
        public UserState() { }

        public UserState(string userId, string state)
        {
            PartitionKey = state;
            RowKey = state;

            UserId = userId;
            State = state;
            CreatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// State Token
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The date this entity was first created
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
