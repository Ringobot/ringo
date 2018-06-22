using System;
using System.Collections.Generic;
using System.Text;
using Ringo.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Ringo.Common.Services
{
    public class ArtistService : IArtistService
    {
        public bool FindArtistMatch(Artist artist)
        {
            throw new NotImplementedException();
        }

        public string GetArtist(string artistId)
        {
            throw new NotImplementedException();
        }

        public Artist GetArtistByUri(string artistUri)
        {
            throw new NotImplementedException();
        }

        public Artist[] GetRelatedArtists(string artist)
        {
            throw new NotImplementedException();
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
                JObject baseArtistProps = new JObject();
                Entity baseArtistEntity = new Entity(baseArtistId, baseArtist.name, baseArtistProps);
                foreach (Artist relatedArtist in relatedArtists.artists)
                {
                    RdostrId relatedArtistRdo = CanonicalService.GetArtistId(relatedArtist.name);
                    string relatedArtistId = $"{relatedArtist.name}:{relatedArtistRdo.Id}";
                    JObject relatedArtistProps = new JObject();
                    Entity relatedEntity = new Entity(relatedArtistId, relatedArtist.name, relatedArtistProps);
                    EntityRelationship entityRelationship = new EntityRelationship();
                    entityRelationship.FromVertex = baseArtistEntity;
                    entityRelationship.Relationship = "related";
                    entityRelationship.RelationshipDate = DateTime.UtcNow;
                    entityRelationship.ToVertex = relatedEntity;
                    entityRelationshipList.Add(entityRelationship);
                }

                
            }
            catch
            {

            }

            return entityRelationshipList;
        }

        public Artist[] SearchArtists(string artist, int limit = 3)
        {
            throw new NotImplementedException();
        }

        Artists IArtistService.GetRelatedArtists(string artist)
        {
            throw new NotImplementedException();
        }

        Artists IArtistService.SearchArtists(string artist, int limit)
        {
            throw new NotImplementedException();
        }
    }
}
