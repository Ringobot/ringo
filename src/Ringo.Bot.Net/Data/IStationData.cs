using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingoBotNet.Data
{
    public interface IStationData
    {
        Task SaveStationUri(string stationId, string channelUserId, string uri, string hashtag = null);
    }
}
