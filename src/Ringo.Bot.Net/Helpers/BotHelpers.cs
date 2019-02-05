using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingoBotNet.Helpers
{
    public static class BotHelpers
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
    }
}
