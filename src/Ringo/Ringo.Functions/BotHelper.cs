using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json.Linq;

namespace Ringo.Functions
{
    class BotHelper
    {
        private static string directLineSecret = Environment.GetEnvironmentVariable("RingoDirectLine");
        private static HttpClient client = new HttpClient();
        private static string tokenUrl = "https://accounts.spotify.com/api/token";
        private static string authUrl = "https://accounts.spotify.com/authorize";
        private static string botId = Environment.GetEnvironmentVariable("RingoBotId");
        private static string fromUser = "DirectLineSampleClientUser";

        public static async Task BuildArtistGraph()
        {
            dynamic artlistList = await GraphHelper.GetArtistRelatedLessThanTwo();
            await LikeArtist(artlistList);

        }

        public static async Task LikeArtist(dynamic artlistList)
        {
            DirectLineClient client = new DirectLineClient(directLineSecret);

            var conversation = await client.Conversations.StartConversationAsync();

            new System.Threading.Thread(async () => await ReadBotMessagesAsync(client, conversation.ConversationId)).Start();

        foreach (var spotifyId in artlistList)
            {
                Console.WriteLine(spotifyId);
                Activity userMessage = new Activity
                {
                    From = new ChannelAccount(fromUser),
                    Text = $"i like {spotifyId}",
                    Type = ActivityTypes.Message
                };

                var msg = await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
                msg.Id.ToString();
            }
        }

    private static async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId)
    {
        string watermark = null;

        while (true)
        {
            var activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);
            watermark = activitySet?.Watermark;

            var activities = from x in activitySet.Activities
                             where x.From.Id == botId
                             select x;

            var str = "";

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
        }
    }
}


    public class Message
    {
        public string type { get; set; }
        public Message from { get; set; }
        public string text { get; set; }
    }

    public class MessageFrom
    {
        public string id { get; set; }
    }

}
