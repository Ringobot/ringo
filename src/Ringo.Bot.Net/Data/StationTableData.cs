using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public class StationTableData : IStationData
    {
        public Task SaveStationUri(string stationId, string channelUserId, string uri, string hashtag = null)
        {
            throw new NotImplementedException();
        }
    }
}
