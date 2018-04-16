using System;
using System.Collections.Generic;

namespace Ringo.Common.Models
{
    public class Entity
    {
        public Entity(string id, string name, Dictionary<string, string> properties)
        {
            this.Id = id;
            this.CreateDate = DateTime.UtcNow;
            this.Name = name;
            this.Properties = properties;
        }

        public string Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Properties { get; set; }


    }
}
