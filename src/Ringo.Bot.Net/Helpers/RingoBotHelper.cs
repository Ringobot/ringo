using Microsoft.Bot.Builder;
using RingoBotNet.Models;
using System;
using System.Text.RegularExpressions;

namespace RingoBotNet.Helpers
{
    public static class RingoBotHelper
    {
        /// <summary>
        /// The system name for Ringobot. This should never change. For bot user name, see <see cref="ConversationInfo"/>
        /// </summary>
        internal const string RingoBotName = "ringo";

        internal const string ChannelIdEmulator = "emulator";
        internal const string ChannelIdSkype = "skype";
        internal const string ChannelIdSlack = "slack";
        internal const string ChannelIdTeams = "msteams";

        internal static string[] SupportedChannelIds = new[]
        {
            ChannelIdEmulator,
            ChannelIdSkype,
            ChannelIdSlack,
            ChannelIdTeams
        };

        internal static readonly Regex NonWordRegex = new Regex("\\W");

        /// <summary>
        /// Used to remove any non alpha-numeric chars (including underscore).
        /// </summary>
        internal static readonly Regex HashtagRegex = new Regex("[^a-zA-Z0-9]");

        public static string ChannelUserId(ITurnContext context)
            => ChannelUserId(NormalizedConversationInfo(context));

        public static string ChannelUserId(ConversationInfo info)
            => User.EncodeIds(info).id;

        public static string LowerWord(string word) => HashtagRegex.Replace(word, string.Empty).ToLower();

        public static ConversationInfo NormalizedConversationInfo(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;

            var info = new ConversationInfo
            {
                ChannelId = activity.ChannelId.ToLower(),
                ChannelTeamId = null,
                FromId = activity.From.Id,
                FromName = activity.From.Name,
                RecipientId = activity.Recipient.Id,
                RecipientName = activity.Recipient.Name,
                ConversationId = activity.Conversation?.Id,
                ConversationName = activity.Conversation?.Name,
                IsGroup = activity.Conversation?.IsGroup ?? false
            };

            // channel specific overrides
            switch (info.ChannelId)
            {
                case ChannelIdSlack:
                    string[] ids = activity.Conversation.Id.Split(':');

                    if (ids.Length < 2) throw new InvalidOperationException("Expecting Conversation Id like BBBBBBBBB:TTTTTTTTTT:CCCCCCCCCC");

                    // ChannelTeamId
                    info.ChannelTeamId = ids[1];

                    // ConversationId
                    if (ids.Length > 2) info.ConversationId = ids[2];

                    break;

                case ChannelIdTeams:
                    dynamic teamsChannelData = activity.ChannelData;
                    info.ChannelTeamId = teamsChannelData.tenant?.id;

                    break;
            }

            return info;
        }

        private static string TeamChannelPart(ConversationInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(info.ChannelId)) throw new ArgumentNullException(nameof(info.ChannelId));
            string teamChannel = CryptoHelper.Base62Encode(info.ChannelId);
            if (!string.IsNullOrEmpty(info.ChannelTeamId)) teamChannel += $"/{CryptoHelper.Base62Encode(info.ChannelTeamId)}";
            return teamChannel;
        }

        public static string ToHashtag(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            return NonWordRegex.Replace(name, string.Empty);
        }

        public static string RingoHandleIfGroupChat(ITurnContext turnContext)
            => RingoHandleIfGroupChat(NormalizedConversationInfo(turnContext));

        public static string RingoHandleIfGroupChat(ConversationInfo info)
            => info.IsGroup ? $"@{info.BotName} " : string.Empty;
    }
}
