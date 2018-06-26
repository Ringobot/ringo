using System;
using System.Collections.Generic;
using System.Text;
using Ringo.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Ringo.Common.Heplers;

namespace Ringo.Common.Services
{
    public class ArtistService : IArtistService
    {
        SpotifyService SpotifyService = new SpotifyService();
        
        public async Task<Tuple<bool, List<Artist>>> FindArtistMatch(string artist)
        {
            bool result = false;
            List<Artist> artistsList = new List<Artist>();
            var artistRequest = await SpotifyService.SearchArtists(artist);
            var artists = MapToArtist(artistRequest);
            foreach (Artist a in artists)
            {
                if (a.image.url.Length > 0)
                {
                    result = true;
                    artistsList.Add(a);

                }
            }
            return Tuple.Create(result, artistsList);

        }

        public async Task<List<Artist>> SearchArtists(string artist, int limit = 3)
        {
            var artistRequest = await SpotifyService.SearchArtists(artist, limit);
            var artists = MapToArtist(artistRequest);
            return artists;
        }

        public async Task<List<Artist>> GetArtist(string artistId)
        {
            try
            {
                var artistRequest = await SpotifyService.GetArtist(artistId);
                var artists = MapToArtist(artistRequest);
                return artists;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<List<Artist>> GetArtistByUri(string artistUri)
        {
            try
            {
                string[] artist = artistUri.Split(new[] { ':' });
                var artistRequest = await SpotifyService.GetRelatedArtists(artist[2]);
                var artists = MapToArtist(artistRequest);
                return artists;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<List<Artist>> GetRelatedArtists(string artistId)
        {
            try
            {
                var artistRequest = await SpotifyService.GetRelatedArtists(artistId);
                var artists = MapToArtist(artistRequest);
                return artists;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public List<Artist> MapToArtist(List<dynamic> data)
        {
            List<Artist> artists = new List<Artist>();
            try
            {
                foreach (var a in data)
                {
                    Artist artist = new Artist()
                    {
                        name = a.name,
                        spotify = new Spotify()
                        {
                            id = a.spotify.id,
                            uri = a.spotify.uri
                        },
                        image = new Image()
                        {
                            height = a.image.height,
                            width = a.image.width,
                            url = a.image.url
                        }

                    };
                    artists.Add(artist);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return artists;

        }

        public List<EntityRelationship> PushRelatedArtist(Artist baseArtist, List<Artist> relatedArtists)
        {
            List<EntityRelationship> entityRelationshipList = new List<EntityRelationship>();
            try
            {
                RdostrId baseArtistRdo = CanonicalService.GetArtistId(baseArtist.name);
                string baseArtistId = $"{baseArtist.name}:{baseArtistRdo.Id}";
                JObject baseArtistProps = JObject.FromObject(new {
                    Properties = new
                    {
                        type = "artist",
                        spotifyid = baseArtist.spotify.id,
                        spotifyuri = baseArtist.spotify.uri,
                        images = baseArtist.image.url
                    }
                });
                Entity baseArtistEntity = new Entity(baseArtistId, baseArtist.name, baseArtistProps);
                foreach (Artist relatedArtist in relatedArtists)
                {
                    RdostrId relatedArtistRdo = CanonicalService.GetArtistId(relatedArtist.name);
                    string relatedArtistId = $"{relatedArtist.name}:{relatedArtistRdo.Id}";
                    JObject relatedArtistProps = JObject.FromObject(new
                    {
                        Properties = new
                        {
                            type = "artist",
                            spotifyid = relatedArtist.spotify.id,
                            spotifyuri = relatedArtist.spotify.uri,
                            images = relatedArtist.image.url
                        }
                    });
                    Entity relatedEntity = new Entity(relatedArtistId, relatedArtist.name, relatedArtistProps);
                    EntityRelationship entityRelationship = new EntityRelationship
                    {
                        FromVertex = baseArtistEntity,
                        Relationship = "related",
                        RelationshipDate = DateTime.UtcNow,
                        ToVertex = relatedEntity
                    };
                    entityRelationshipList.Add(entityRelationship);
                }

                
            }
            catch
            {

            }

            return entityRelationshipList;
        }

    }
}
