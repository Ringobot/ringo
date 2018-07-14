using Ringo.Common.Models;

namespace Ringo.Common.Services
{
    public interface ICanonicalService
    {
        RdostrId GetArtistId(string input);
    }
}