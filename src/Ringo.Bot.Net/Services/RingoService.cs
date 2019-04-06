using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Data;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public class RingoService : IRingoService
    {
        private readonly IConfiguration _config;
        private readonly IUserData _userData;
        private IStationData _stationData;
        private readonly ILogger _logger;

        public RingoService(
            IConfiguration configuration,
            IUserData channelUserData,
            IStationData stationData,
            ILogger<RingoService> logger
            )
        {
            _config = configuration;
            _userData = channelUserData;
            _stationData = stationData;
            _logger = logger;
        }

        public async Task<User> CreateUserIfNotExists(string channelId, string userId, string username)
        {
            return await _userData.CreateUserIfNotExists(channelId, userId, username);
        }

        public async Task<Station> GetUserStation(ConversationInfo info, string username)
            => await _stationData.GetStation(RingoBotHelper.ToUserStationUri(info, username));

        public async Task<Station> GetChannelStation(ConversationInfo info, string conversationName = null)
        {
            if (!info.IsGroup && string.IsNullOrEmpty(conversationName))
            {
                throw new ArgumentException("Must provide conversationName if not in group chat", nameof(conversationName));
            }

            return await _stationData.GetStation(RingoBotHelper.ToChannelStationUri(info, conversationName));
        }

        private async Task<Station> CreateStation(
            string userId,
            string uri,
            string hashtag,
            Album album = null,
            Playlist playlist = null)
        {
            // get user
            var user = await _userData.GetUser(userId);

            // get station
            var station = await _stationData.GetStation(uri);

            // save station
            if (station == null)
            {
                // new station
                station = await _stationData.CreateStation(uri, user, album, playlist, hashtag);
            }
            else
            {
                // update station context and owner
                station.Name = album?.Name ?? playlist?.Name;
                station.Owner = user;
                station.Album = album;
                station.Playlist = playlist;
                station.Hashtag = hashtag;

                if (!station.ActiveListeners.Any(l => l.User.Id == userId))
                {
                    // add the new owner to the listeners
                    station.ActiveListeners = new List<Listener>(station.ActiveListeners)
                    {
                        new Listener(station, user)
                    }.ToArray();
                }

                await _stationData.ReplaceStation(uri, station);
            }

            return station;
        }

        public async Task<Station> CreateChannelStation(
            string channelUserId,
            ConversationInfo info,
            Models.Album album = null,
            Models.Playlist playlist = null)
            => await CreateStation(
                channelUserId,
                RingoBotHelper.ToChannelStationUri(info),
                RingoBotHelper.ToHashtag(info.ConversationName),
                album: album,
                playlist: playlist);

        public async Task<Station> CreateUserStation(
            string channelUserId,
            ConversationInfo info,
            Models.Album album = null,
            Models.Playlist playlist = null)
            => await CreateStation(
                channelUserId,
                RingoBotHelper.ToUserStationUri(info, info.FromName),
                RingoBotHelper.ToHashtag(info.FromName),
                album: album,
                playlist: playlist);
    }
}
