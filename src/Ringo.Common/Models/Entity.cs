using System;

namespace Ringo.Common.Models
{
    public class Entity
    {
        public Entity(string id, string name, string type)
        {
            this.id = id;
            this.createDate = DateTime.UtcNow;
            this.name = name;
            this.type = type;
        }

        public string id { get; set; }
        public DateTime createDate { get; set; }
        public string name { get; set; }
        public string type { get; set; }


    }
}
