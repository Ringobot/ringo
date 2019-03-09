using RingoBotNet.Models;
using System.Linq;

namespace RingoBotNet.Mappers
{
    public static class ItemMappers
    {
        public static Album MapToAlbum(SpotifyApi.NetCore.Album album)
        {
            if (album == null) return null;

            var item = new Album
            {
                Id = album.Id,
                Name = album.Name,
                Uri = album.Uri,
                Href = album.Href.ToString(),
                ExternalUrls = new ExternalUrls { Spotify = album.ExternalUrls.Spotify },
                Artists = album.Artists.Select(MapToArtist).ToArray()
            };

            if (album.Images != null && album.Images.Any())
            {
                item.Images = album.Images.Select(i => new Image
                {
                    Height = i.Height,
                    Url = i.Url,
                    Width = i.Width
                }).ToArray();
            }

            return item;
        }

        public static Artist MapToArtist(SpotifyApi.NetCore.Artist artist)
        {
            if (artist == null) return null;

            var item = new Artist
            {
                Id = artist.Id,
                Name = artist.Name,
                Uri = artist.Uri,
                Href = artist.Href,
                ExternalUrls = new ExternalUrls { Spotify = artist.ExternalUrls.Spotify }
            };

            if (artist.Images != null && artist.Images.Any())
            {
                item.Images = artist.Images.Select(i => new Image
                {
                    Height = i.Height,
                    Url = i.Url,
                    Width = i.Width
                }).ToArray();
            }

            return item;
        }

        public static Playlist MapToPlaylist(SpotifyApi.NetCore.PlaylistSimplified playlist)
        {
            if (playlist == null) return null;

            var item = new Playlist
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Uri = playlist.Uri,
                Href = playlist.Href,
                ExternalUrls = new ExternalUrls { Spotify = playlist.ExternalUrls.Spotify }
            };

            if (playlist.Images != null && playlist.Images.Any())
            {
                item.Images = playlist.Images.Select(i => new Image
                {
                    Height = i.Height,
                    Url = i.Url,
                    Width = i.Width
                }).ToArray();
            }

            return item;
        }
    }
}
