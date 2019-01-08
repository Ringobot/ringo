using System;

namespace Ringo.Bot.Net.State
{
    /// <summary>
    /// User state is available in any turn that the bot is conversing with that user on that channel, regardless of the conversation.
    /// </summary>
    public class UserProfile
    {
        public DateTime FooBar { get; set; }
    }
}
