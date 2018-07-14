using Ringo.Common.Models;
using System;

namespace Ringo.Common.Helpers
{
    public class ArtistHelper
    {
        public static Artist MapToArtist(dynamic data)
        {
            try
            {
                Artist artist = new Artist()
                {
                    name = data.name,
                    spotify = new Spotify()
                    {
                        id = data.id,
                        uri = data.uri
                    },
                    images = data.images.ToObject<Image[]>()
                };

                return artist;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
