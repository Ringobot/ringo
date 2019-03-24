using Microsoft.Bot.Builder;
using RingoBotNet.Models;
using System;
using System.Text.RegularExpressions;

namespace RingoBotNet.Helpers
{
    public static class RingoBotHelper
    {
        //public const string RingoBotName = "ringo";
        public static readonly Regex NonWordRegex = new Regex("\\W");

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
                ChannelTeamId = activity.ChannelId.ToLower(),
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

        public static string ToUserStationUri(ConversationInfo info, string username)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

            // ringo:slack/TA0VBN61L:station:user/daniel312
            return $"{info.BotName}:{TeamChannelPart(info)}:station:user/{NonWordRegex.Replace(username.ToLower(), string.Empty)}";
        }

        public static string ToHashtagStationUri(ConversationInfo info, string hashtag)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(hashtag)) throw new ArgumentNullException(nameof(hashtag));

            // ringo:twitter:station:hashtag/datenight
            return $"{info.BotName}:{TeamChannelPart(info)}:station:hashtag/{ToHashtag(hashtag).ToLower()}";
        }

        public static string ToChannelStationUri(ConversationInfo info, string conversationName = null)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(conversationName) && string.IsNullOrEmpty(info.ConversationName))
                throw new ArgumentNullException(nameof(conversationName));

            // ringo:slack/TA0VBN61L:station:channel/testing3
            return $"{info.BotName}:{TeamChannelPart(info)}:station:channel/{ToHashtag(conversationName ?? info.ConversationName).ToLower()}";
        }

        private static string TeamChannelPart(ConversationInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(info.ChannelId)) throw new ArgumentNullException(nameof(info.ChannelId));
            if (string.IsNullOrEmpty(info.ChannelTeamId)) throw new ArgumentNullException(nameof(info.ChannelTeamId));

            return $"{info.ChannelId}/{info.ChannelTeamId}";
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
