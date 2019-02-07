using Newtonsoft.Json;
using SpotifyApi.NetCore;

namespace RingoBotNet.Models
{
    public class Playlist
    {
        //public ExternalUrls ExternalUrls { get; set; }

        //public string Href { get; set; }

        public string Id { get; set; }

        //public Image[] Images { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }
    }

    //public partial class ExternalUrls
    //{
    //    public string Spotify { get; set; }
    //}

    //public partial class Image
    //{
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    //    public int Height { get; set; }

    //    public string Url { get; set; }

    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    //    public int Width { get; set; }
    //}
}