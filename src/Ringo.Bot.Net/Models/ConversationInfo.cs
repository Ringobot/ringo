using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingoBotNet.Models
{
    public class ConversationInfo
    {
        public string ChannelId { get; set; }

        public string ChannelTeamId { get; set; }

        public string FromId { get; set; }

        public string FromName { get; set; }

        public string RecipientId { get; set; }

        public string RecipientName { get; set; }

        public string ConversationId { get; set; }

        public string ConversationName { get; set; }

        public bool IsGroup { get; set; }

        public string ChannelUserId { get; set; }
    }
}
