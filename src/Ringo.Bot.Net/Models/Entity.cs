using Newtonsoft.Json;

namespace RingoBotNet.Models
{
    public abstract class Entity
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
