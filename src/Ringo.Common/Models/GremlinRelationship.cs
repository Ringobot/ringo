using System;

namespace Ringo.Common.Models
{
    public class GremlinRelationship
    {
        public string FromVertex { get; set; }
        public string Relationship { get; set; }
        public string ToVertex { get; set; }
        public DateTime RelationshipDate { get; set; }
    }
}
