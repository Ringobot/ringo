using System;
using System.Collections.Generic;

namespace Ringo.Common.Models
{
    public class Entity
    {
        public Entity(string id, string name, Dictionary<string, string> properties)
        {
            this.id = id;
            this.createDate = DateTime.UtcNow;
            this.name = name;
            this.properties = properties;
        }

        public string id { get; set; }
        public DateTime createDate { get; set; }
        public string name { get; set; }
        public Dictionary<string, string> properties { get; set; }


    }
}
