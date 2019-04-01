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
        public const string RingoBotName = "ringo";

        public static readonly Regex NonWordRegex = new Regex("\\W");

        public static readonly Regex HashtagRegex = new Regex("[^a-zA-Z0-9]");

        public static string ChannelUserId(ITurnContext context)
            => ChannelUserId(context.Activity.ChannelId, context.Activity.From.Id);

        public static string ChannelUserId(string channelId, string userId)
            => ChannelUser.EncodeId(channelId, userId);

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
                case "slack":
                    string[] ids = activity.Conversation.Id.Split(':');

                    if (ids.Length < 2) throw new InvalidOperationException("Expecting Conversation Id like BBBBBBBBB:TTTTTTTTTT:CCCCCCCCCC");

                    info.ChannelTeamId = ids[1];
                    if (ids.Length > 2) info.ConversationId = ids[2];

                    break;

                case "msteams":
                    dynamic teamsChannelData = activity.ChannelData;
                    info.ChannelTeamId = teamsChannelData.tenant?.id;

                    break;

                //case "emulator":
                //    dynamic emulatorChannelData = activity.ChannelData;

                //    break;
            }

            return info;
        }

        public static string ToUserStationUri(ConversationInfo info, string username, string hashtag = null)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

            // ringo:{tenant62}:station:user:{username62}[:hashtag:{hashtag}]
            string uri = $"{RingoBotName}:{TeamChannelPart(info)}:station:user:{CryptoHelper.Base62Encode(NonWordRegex.Replace(username.ToLower(), string.Empty))}";
            if (!string.IsNullOrEmpty(hashtag)) uri += $":hashtag:{ToHashtag(hashtag).ToLower()}";
            return uri;
        }

        public static string ToChannelStationUri(ConversationInfo info, string conversationName = null, string hashtag = null)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(conversationName) && string.IsNullOrEmpty(info.ConversationName))
                throw new ArgumentNullException(nameof(conversationName));

            // ringo:{tenant62}:station:channel:{channelName62}[:hashtag:{hashtag}]
            string uri = $"{RingoBotName}:{TeamChannelPart(info)}:station:channel:{CryptoHelper.Base62Encode(ToHashtag(conversationName ?? info.ConversationName).ToLower())}";
            if (!string.IsNullOrEmpty(hashtag)) uri += $":hashtag:{ToHashtag(hashtag).ToLower()}";
            return uri;
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
