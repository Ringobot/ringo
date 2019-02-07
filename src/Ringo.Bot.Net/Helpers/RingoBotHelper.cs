using Microsoft.Bot.Builder;
using RingoBotNet.Models;
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
    }
}
