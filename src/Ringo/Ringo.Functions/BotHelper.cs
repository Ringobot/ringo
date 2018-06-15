using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ringo.Functions
{
    class BotHelper
    {
        private static string directLineSecret = Environment.GetEnvironmentVariable("RingoDirectLine");
        private static HttpClient client = new HttpClient();
        private static string fromUser = "DirectLineSampleClientUser";

        public static async Task BuildArtistGraph()
        {
            dynamic artlistList = await GraphHelper.GetArtistRelatedLessThanTwo();
            await LikeArtist(artlistList);

        }

        public static async Task LikeArtist(dynamic artlistList)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", directLineSecret);

            var convIdRequest = await client.PostAsync("https://directline.botframework.com/v3/directline/conversations", null);

            var convIdResult = await convIdRequest.Content.ReadAsStringAsync();
            JObject convIdObj = JObject.Parse(convIdResult);
            string convId = (string)convIdObj["conversationId"];

            foreach (var spotifyId in artlistList)
            {
                Message msg = new Message()
                {
                    type = "message",
                    from = new MessageFrom() { id = fromUser },
                    text = $"i like {spotifyId}"

                };
                var msgJson = JsonConvert.SerializeObject(msg);

                var sendArtisit = await client.PostAsync($"https://directline.botframework.com/v3/directline/conversations/{convId}/activities", new StringContent(msgJson, Encoding.UTF8, "application/json"));

                var sendArtisitResult = await sendArtisit.Content.ReadAsStringAsync();
            }
        }
        
    }

    public class Message
    {
        public string type { get; set; }
        public MessageFrom from { get; set; }
        public string text { get; set; }
    }

    public class MessageFrom
    {
        public string id { get; set; }
    }


}
