using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ringo.Common.Models;
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

                JObject sendArtisitResultObj = JObject.Parse(sendArtisitResult);

                var watermarkString = (string)sendArtisitResultObj["id"];
                    
                var watermarkStringFilter = watermarkString.Substring(watermarkString.IndexOf("|"));

                int watermarkInt;
                int.TryParse(watermarkString, out watermarkInt);

                var getConvResponse = await client.GetAsync($"https://directline.botframework.com/v3/directline/conversations/{convId}/activities?watermark={watermarkInt++}");

                var getConvResult = await getConvResponse.Content.ReadAsStringAsync();

                BotMessage botMessage = JsonConvert.DeserializeObject<BotMessage>(getConvResult);

                foreach (Activity activity in botMessage.activities)
                {
                    if (activity.text == "Oops. Something went wrong and we need to start over.")
                    {
                        throw new Exception("Error from Ringo Bot");
                    }
                }

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
