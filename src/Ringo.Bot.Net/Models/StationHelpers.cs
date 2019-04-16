using RingoBotNet.Helpers;
using System;
using System.Text.RegularExpressions;

namespace RingoBotNet.Models
{
    /// <summary>
    /// Station Domain Model Object Logic and Helpers
    /// </summary>
    public partial class Station
    {
        internal static Regex StationPKRegex = new Regex(
            $"^{RingoBotHelper.RingoBotName}:({string.Join('|', RingoBotHelper.SupportedChannelIds)}):[a-zA-Z0-9=]*:station:(conversation|user):[a-z0-9]+$");
        internal static Regex StationIdRegex = new Regex(
            $"^{RingoBotHelper.RingoBotName}:({string.Join('|', RingoBotHelper.SupportedChannelIds)}):[a-zA-Z0-9=]*:station:(conversation:[a-z0-9]+:hashtag:[a-z0-9]+|user:[a-z0-9]+)$");

        /// <summary>
        /// Encodes the Id and Partition Key into a format suitable for a <see cref="CosmosEntity"/>
        /// </summary>
        /// <param name="hashtag">Hashtag</param>
        /// <param name="info">Conversation Info</param>
        /// <param name="username">Username for User Station. Must be null for Conversation Station</param>
        internal static (string id, string pk) EncodeIds(ConversationInfo info, string hashtag, string username = null)
        {
            if (!string.IsNullOrEmpty(username))
            {
                // User Station
                // ringo:{channel_id}:{channel_team_id.ToLower()}:station:user:{lower_word(user_name)}
                string id = $"{RingoBotHelper.RingoBotName}:{info.ChannelId}:{info.ChannelTeamId}:station:user:{RingoBotHelper.LowerWord(username)}"
                    .ToLower();
                return (id, id);
            }

            // Conversation Station
            // ringo:{channel_id}:{channel_team_id.ToLower()}:station:conversation:{lower_word(conversation_name)}[:hashtag:{lower_word(hashtag)}]
            string pk = $"{RingoBotHelper.RingoBotName}:{info.ChannelId}:{info.ChannelTeamId}:station:conversation:{RingoBotHelper.LowerWord(info.ConversationName)}"
                .ToLower();
            return ($"{pk}:hashtag:{RingoBotHelper.LowerWord(hashtag)}", pk);
        }

        internal override void EnforceInvariants(bool isRoot = false)
        {
            base.EnforceInvariants();
            if (!StationIdRegex.IsMatch(Id)) throw new InvariantException("Station Id is not correct format.");
            if (!StationPKRegex.IsMatch(PartitionKey)) throw new InvariantException("Station Partition Key is not correct format.");
            if (string.IsNullOrEmpty(Hashtag)) throw new InvariantException("Hashtag must not be null.");
            if (string.IsNullOrEmpty(Name)) throw new InvariantException("Name must not be null.");
            if (Album == null && Playlist == null) throw new InvariantException("Station must have Album or Playlist property set.");

            if (Album != null && Playlist != null)
            {
                throw new InvariantException("Station must have only one of Album or Playlist property set.");
            }

            if (ListenerCount < 0) throw new InvariantException("ListenerCount must not be less than Zero.");

            if (isRoot)
            {
                Owner.EnforceInvariants();
                foreach (var listener in ActiveListeners) listener.EnforceInvariants();
            }
        }

    }
}
