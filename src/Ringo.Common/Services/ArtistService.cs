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
        public static SpotifyService SpotifyService = new SpotifyService();
        
        public bool FindArtistMatch(Artist artist)
        {
            throw new NotImplementedException();
        }

        public async Task<Artist> GetArtist(string artistId)
        {
            try
            {
                dynamic artistRequest = await SpotifyService.GetArtist(artistId);
                Artist artist = new Artist()
                {
                    name = artistRequest.name,
                };
                return artist;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<Artist> GetArtistByUriAsync(string artistUri)
        {
            return await SpotifyService.GetArtist(artistUri);
        }

        public async Task<Artists> GetRelatedArtistsAsync(string artistId)
        {
            return await SpotifyService.GetRelatedArtists(artistId);

        }

        public Artists MapToArtist(string data)
        {
            try
            {
                Artists artist = JsonConvert.DeserializeObject<Artists>(data);
                return artist;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public List<EntityRelationship> PushRelatedArtist(Artist baseArtist, Artists relatedArtists)
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
                        images = baseArtist.image[0].url
                    }
                });
                Entity baseArtistEntity = new Entity(baseArtistId, baseArtist.name, baseArtistProps);
                foreach (Artist relatedArtist in relatedArtists.artists)
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
                            images = relatedArtist.image[0].url
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

        public async Task<Artists> SearchArtists(string artist, int limit = 3)
        {
            throw new NotImplementedException();
        }


    }
}
