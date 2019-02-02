using System;
using System.Collections.Generic;
using Microsoft.Bot.Schema;

namespace RingoBotNet.State
{
    /// <summary>
    /// Conversation state is available in any turn in a specific conversation, regardless of user (i.e. group conversations).
    /// </summary>
    public class ConversationData
    {
        public ConversationData()
        {
            ConversationUserTokens = new Dictionary<string, TokenResponse>();
            UserStateTokens = new Dictionary<string, string>();
        }

        public Dictionary<string, TokenResponse> ConversationUserTokens { get; set; }

        public Dictionary<string, string> UserStateTokens { get; set; }
    }
}
