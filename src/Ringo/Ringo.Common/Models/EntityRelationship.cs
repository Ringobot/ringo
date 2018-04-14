using System;

namespace Ringo.Common.Models
{
    public class EntityRelationship
    {
        public Entity FromVertex { get; set; }
        public string Relationship { get; set; }
        public Entity ToVertex { get; set; }
        public DateTime RelationshipDate { get; set; }
    }
}
