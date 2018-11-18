using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Ringo.Bot.Net
{
    /// <summary>
    /// This is a helper class to support the state accessors for the bot.
    /// </summary>
    public class RingoBotAccessors
    {
        // The name of the dialog state.
        public static readonly string DialogStateName = $"{nameof(RingoBotAccessors)}.DialogState";

        /// <summary>
        /// Gets or Sets the DialogState accessor value.
        /// </summary>
        /// <value>
        /// A <see cref="DialogState"/> representing the state of the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
    }
}
