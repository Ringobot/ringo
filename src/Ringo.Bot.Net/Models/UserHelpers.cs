using RingoBotNet.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RingoBotNet.Models
{
    public partial class User
    {
        internal static Regex UserIdRegex = new Regex($"^{RingoBotHelper.RingoBotName}:(slack|msteams|skype):[a-zA-Z0-9=]*:user:[a-z0-9-:]+$");

        /// <summary>
        /// Encodes the Id and Partition Key into a format suitable for a <see cref="CosmosEntity"/>
        /// </summary>
        /// <param name="channelId">A Channel Id that identifies the Bot channel, i.e. "msteams", "slack".</param>
        /// <param name="userId">The User Id provided by the Bot Channel</param>
        /// <returns>An Id, PK tuple.</returns>
        internal static (string id, string pk) EncodeIds(ConversationInfo info, string userId)
        {
            // ringo:{channel_id}:{channel_team_id.ToLower()}:user:user_id.ToLower()}
            string id = $"{RingoBotHelper.RingoBotName}:{info.ChannelId}:{info.ChannelTeamId}:user:{RingoBotHelper.LowerWord(userId)}"
                .ToLower();
            return (id, id);
        }

        internal static (string id, string pk) EncodeIds(ConversationInfo info) 
            => EncodeIds(info, info.FromId);

        internal override void EnforceInvariants(bool isRoot = false)
        {
            base.EnforceInvariants();
            if (!UserIdRegex.IsMatch(Id)) throw new InvariantException("User Id is not correct format.");
            if (!UserIdRegex.IsMatch(PartitionKey)) throw new InvariantException("User Partition Key is not correct format.");
            if (Type != TypeName) throw new InvariantException("Type must not be null or empty");
            if (string.IsNullOrEmpty(UserId)) throw new InvariantException("UserId must not be null or empty");
            if (string.IsNullOrEmpty(ChannelId)) throw new InvariantException("ChannelId must not be null or empty");
        }
    }
}
