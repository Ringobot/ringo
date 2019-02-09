using Microsoft.Bot.Builder;
using RingoBotNet.Models;
using System;
using System.Text.RegularExpressions;

namespace RingoBotNet.Helpers
{
    public static class RingoBotHelper
    {
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
            }

            return info;
        }

        public static string ToUserStationUri(ConversationInfo info, string username) 
            => $"{TeamChannelPart(info)}/@{username.ToLower()}";

        public static string ToHashtagStationUri(ConversationInfo info, string hashtag)
            => $"{TeamChannelPart(info)}/#{ToHashtag(hashtag).ToLower()}";

        public static string ToConversationStationUri(ConversationInfo info) 
            => $"{TeamChannelPart(info)}/#{ToHashtag(info.ConversationName).ToLower()}/{info.ConversationId}";

        private static string TeamChannelPart(ConversationInfo info) => $"{info.ChannelId}/{info.ChannelTeamId}";

        public static string ToHashtag(string name) => NonWordRegex.Replace(name, string.Empty);
    }
}
