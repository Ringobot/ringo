using System;
using Newtonsoft.Json.Linq;

namespace Ringo.Common.Models
{
    public class Entity
    {
        public Entity(string id, string name, JObject properties)
        {
            this.Id = id;
            this.CreateDate = DateTime.UtcNow;
            this.Name = name;
            this.Properties = properties;
        }

        public string Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Name { get; set; }
        public JObject Properties { get; set; }


    }
}
