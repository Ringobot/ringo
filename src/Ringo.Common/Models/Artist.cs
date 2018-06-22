using System;
using System.Collections.Generic;
using System.Text;

namespace Ringo.Common.Models
{

    public class Artists
    {
        public List<Artist> artists { get; set; }
    }

    public class Artist
    {
        public string name { get; set; }
        public Spotify spotify { get; set; }
        public Image[] image { get; set; }
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
