﻿using Microsoft.Extensions.Configuration;
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
        private readonly IStationData _stationData;
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

        public async Task<User> CreateUserIfNotExists(ConversationInfo info, string userId = null, string username = null)
        {
            return await _userData.CreateUserIfNotExists(info, userId, username);
        }

        public async Task<Station> GetUserStation(ConversationInfo info, string username)
            => await _stationData.GetStation(Station.EncodeIds(info, RingoBotHelper.ToHashtag(username), username));

        public async Task<Station> GetChannelStation(ConversationInfo info, string conversationName = null)
        {
            if (!info.IsGroup && string.IsNullOrEmpty(conversationName))
            {
                throw new ArgumentException("Must provide conversationName if not in group chat", nameof(conversationName));
            }

            return await _stationData.GetStation(Station.EncodeIds(info, RingoBotHelper.ToHashtag(conversationName)));
        }

        private async Task<Station> CreateStation(
            ConversationInfo info,
            string hashtag,
            string ownerUserId,
            string username = null,
            Album album = null,
            Playlist playlist = null)
        {
            // get user
            var ownerUser = await _userData.GetUser(ownerUserId);

            // get station
            var stationIds = Station.EncodeIds(info, hashtag);
            var station = await _stationData.GetStation(stationIds);

            // save station
            if (station == null)
            {
                // new station
                station = new Station(info, hashtag, album, playlist, ownerUser);
                await _stationData.CreateStation(station);
            }
            else
            {
                // update station context and owner
                station.Name = album?.Name ?? playlist?.Name;
                station.Owner = ownerUser;
                station.Album = album;
                station.Playlist = playlist;
                station.Hashtag = hashtag;

                if (!station.ActiveListeners.Any(l => l.User.Id == ownerUserId))
                {
                    // add the new owner to the listeners
                    station.ActiveListeners = new List<Listener>(station.ActiveListeners)
                    {
                        new Listener(station, ownerUser)
                    }.ToArray();
                }

                await _stationData.ReplaceStation(station);
            }

            return station;
        }

        public async Task<Station> CreateConversationStation(
            ConversationInfo info,
            Album album = null,
            Playlist playlist = null)
        {
            string hashtag = RingoBotHelper.ToHashtag(album?.Name ?? playlist?.Name);

            return await CreateStation(
                           info,
                           hashtag,
                           User.EncodeIds(info).id,
                           album: album,
                           playlist: playlist);
        }

        public async Task<Station> CreateUserStation(
            ConversationInfo info,
            Album album = null,
            Playlist playlist = null)
            => await CreateStation(
                info,
                RingoBotHelper.ToHashtag(info.FromName),
                User.EncodeIds(info).id,
                username: info.FromName,
                album: album,
                playlist: playlist);
    }
}
