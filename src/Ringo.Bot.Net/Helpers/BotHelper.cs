using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingoBotNet.Helpers
{
    public static class BotHelper
    {
        public static ChannelAccount GetFirstMentioned(ITurnContext turnContext)
        {
            // user mentioned?
            Mention mention = null;

            if (turnContext.Activity.Entities != null)
            {
                var mentions = turnContext.Activity.Entities.Where(e => e.Type == "mention").Select(e => e.GetAs<Mention>());
                mention = mentions.Where(m => m.Mentioned.Id != turnContext.Activity.Recipient.Id).FirstOrDefault();
            }

            if (mention == null || mention.Mentioned == null) return null;

            return mention.Mentioned;
        }

        public static bool NotListening(ITurnContext turnContext)
        {
            // if in a group and the bot is not mentioned, ignore this dialog
            return (IsGroup(turnContext) && !IsMentioned(turnContext));
        }

        public static bool IsGroup(ITurnContext turnContext)
        {
            return turnContext.Activity.Conversation != null
                && turnContext.Activity.Conversation.IsGroup.HasValue
                && turnContext.Activity.Conversation.IsGroup.Value;
        }

        public static bool IsMentioned(ITurnContext turnContext)
        {
            if (turnContext.Activity.Entities == null) return false;
            var mentions = turnContext.Activity.Entities.Where(e => e.Type == "mention").Select(e => e.GetAs<Mention>());
            return mentions.Any(m => m.Mentioned.Id == turnContext.Activity.Recipient.Id);
        }

        public static string TokenForLogging(string token)
        {
            if (token == null) return null;
            if (token.Length <= 5) return token;
            return $"{token.Substring(0, 5)}...";
        }
    }
}
