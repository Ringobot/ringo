using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Ringo.Bot.Net.State
{
    /// <summary>
    /// This is a helper class to support the state accessors for the bot.
    /// </summary>
    public class RingoBotAccessors2
    {
        // The name of the dialog state.
        public static readonly string DialogStateName = $"{nameof(RingoBotAccessors)}.{nameof(DialogState)}";
        public static readonly string UserStateName = $"{nameof(RingoBotAccessors)}.{nameof(UserState)}";

        /// <summary>
        /// Gets or Sets the DialogState accessor value.
        /// </summary>
        /// <value>
        /// A <see cref="DialogState"/> representing the state of the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> DialogState { get; set; }

        public IStatePropertyAccessor<UserProfile> UserState { get; set; }
    }

    public class RingoBotAccessors
    {
        public RingoBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        public static readonly string DialogStateName = $"{nameof(RingoBotAccessors)}.{nameof(DialogState)}";

        public static string UserProfileName { get; } = "UserProfile";

        public static string ConversationDataName { get; } = "ConversationData";

        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }

        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }

        public IStatePropertyAccessor<DialogState> DialogState { get; set; }

        public ConversationState ConversationState { get; }

        public UserState UserState { get; }
    }
}
