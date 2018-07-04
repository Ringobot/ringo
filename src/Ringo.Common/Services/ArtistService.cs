using Newtonsoft.Json.Linq;
using Ringo.Common.Models;
using Ringo.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ringo.Common.Services
{
    public class ArtistService : IArtistService
    {
        private static SpotifyService spotifyService;

        public ArtistService()
        {
            spotifyService = new SpotifyService();
        }

        public async Task<Artist> GetArtist(string artistId)
        {
            try
            {
                var artists = await spotifyService.GetArtist(artistId);
                var result = ArtistHelper.MapToArtist(artists);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<Artist> GetArtistByUri(string artistUri)
        {
            try
            {
                string[] artist = artistUri.Split(new[] { ':' });
                var artistRequest = await spotifyService.GetArtist(artist[2]);
                var artists = ArtistHelper.MapToArtist(artistRequest);
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
                List<Artist> artistsList = new List<Artist>();
                dynamic artistRequest = await spotifyService.GetRelatedArtists(artistId);
                foreach (var item in artistRequest.artists)
                {
                    artistsList.Add(ArtistHelper.MapToArtist(item));
                }
                return artistsList;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

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
                        images = baseArtist.images[0]
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
                            images = relatedArtist.images[0]
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

        public async Task<List<Artist>> SearchArtists(string artist, int limit = 3)
        {
            List<Artist> artistsList = new List<Artist>();
            dynamic artistRequest = await spotifyService.SearchArtists(artist);
            foreach (var item in artistRequest.artists.items)
            {
                artistsList.Add(ArtistHelper.MapToArtist(item));
            }
            return artistsList;
        }

    }
}
