namespace Ringo.Common.Models
{

    public class Artist
    {
        public string name { get; set; }
        public Spotify spotify { get; set; }
        public Image[] images { get; set; }
    }

    public class Spotify
    {
        public string id { get; set; }
        public string uri { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public int width { get; set; }
        public string url { get; set; }
    }

}
