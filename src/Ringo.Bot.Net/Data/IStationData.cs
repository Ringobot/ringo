using RingoBotNet.Models;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IStationData
    {
        Task CreateStation(Station station);

        Task ReplaceStation(Station station);

        Task<Station> GetStation((string id, string pk) stationIds);
    }
}
