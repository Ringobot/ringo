using Microsoft.ServiceFabric.Services.Remoting;
using Ringo.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ringo.Common.Interfaces
{

    public interface IRelatedStateful : IService
    {
         Task PushRelatedArtist(string baseArtist, List<Artist> relatedArtists);
    }
}
