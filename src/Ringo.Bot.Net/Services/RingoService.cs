﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RingoBotNet.Data;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Helpers;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public class RingoService : IRingoService
    {
        private static readonly Regex SpotifyPlaylistUrlRegex = new Regex("playlist\\/[a-zA-Z0-9]+");

        private readonly IPlaylistsApi _playlists;
        private readonly IPlayerApi _player;
        private readonly IConfiguration _config;
        private readonly IChannelUserData _userData;
        //private readonly IStationHashcodeData _stationHashcodeData;
        private readonly ILogger _logger;
        private IStationData _stationData;

        public RingoService(
            IPlaylistsApi playlists,
            IPlayerApi player,
            IConfiguration configuration,
            IChannelUserData channelUserData,
            IStationData stationData,
            ILogger<RingoService> logger
            )
        {
            _playlists = playlists;
            _player = player;
            _config = configuration;
            _userData = channelUserData;
            _stationData = stationData;
            _logger = logger;
        }

        public async Task<Models.Playlist[]> FindPlaylists(string searchText, string accessToken, CancellationToken cancellationToken)
        {
            Models.Playlist[] playlists = null;

            string uriOrId = null;

            if (uriOrId == null)
            {
                // spotify:user:daniellarsennz:playlist:5JOGypafQPEx0GkXyLb948
                MatchCollection matchesUserUri = SpotifyUriHelper.SpotifyUserPlaylistUriRegEx.Matches(searchText);
                if (matchesUserUri.Any()) uriOrId = matchesUserUri[0].Value;
            }

            if (uriOrId == null)
            {
                // spotify:playlist:5JOGypafQPEx0GkXyLb948
                MatchCollection matchesUri = SpotifyUriHelper.SpotifyUriRegEx.Matches(searchText);
                if (matchesUri.Any() && SpotifyUriHelper.SpotifyUriType(matchesUri[0].Value) == "playlist")
                {
                    uriOrId = matchesUri[0].Value;
                }
            }

            if (uriOrId == null)
            {
                // https://open.spotify.com/user/daniellarsennz/playlist/3dzMCDJTULeZ7IgbWvotSB?si=bm-3giiVS76AW5yXplr-pQ
                MatchCollection matchesPlaylistUri = SpotifyPlaylistUrlRegex.Matches(searchText);
                if (matchesPlaylistUri.Any()) uriOrId = matchesPlaylistUri[0].Value.Split('/').Last();
            }

            if (uriOrId == null)
            {
                // search for the Playlist
                var results = await RetryHelper.RetryAsync(
                    () => _playlists.SearchPlaylists(searchText, accessToken: accessToken),
                    logger: _logger,
                    cancellationToken: cancellationToken);

                if (results.Total > 0)
                {
                    playlists = results.Items.Take(3).Select(MapToPlaylist).ToArray();
                }
            }
            else
            {
                playlists = new[] { await GetPlaylist(accessToken, uriOrId) };
            }

            return playlists;
        }

        public async Task<Models.Playlist> GetPlaylist(string token, string playlistUriOrId)
        {
            var playlistSimple = await RetryHelper.RetryAsync(() => _playlists.GetPlaylist(playlistUriOrId), logger: _logger);
            return playlistSimple == null ? null : MapToPlaylist(playlistSimple);
        }

        public async Task PlayPlaylist(string playlistId, string accessToken, CancellationToken cancellationToken)
        {
            await RetryHelper.RetryAsync(
                () => _player.PlayPlaylist(playlistId, accessToken),
                logger: _logger,
                cancellationToken: cancellationToken);
        }

        public async Task<Station> CreateChannelStation(
            string channelUserId,
            ConversationInfo info,
            Models.Playlist playlist)
            => await CreateStation(
                channelUserId,
                playlist,
                RingoBotHelper.ToChannelStationUri(info),
                RingoBotHelper.ToHashtag(info.ConversationName));

        public async Task<Station> CreateUserStation(
            string channelUserId,
            ConversationInfo info,
            Models.Playlist playlist)
            => await CreateStation(
                channelUserId,
                playlist,
                RingoBotHelper.ToUserStationUri(info, info.FromName),
                RingoBotHelper.ToHashtag(info.FromName));

        public async Task<Device[]> GetDevices(string accessToken)
        {
            return await _player.GetDevices<Device[]>(accessToken);
        }

        public async Task<string> GetPlaylistTrackOneUrl(string token, Models.Playlist playlist)
        {
            var tracks = await _playlists.GetTracks(playlist.Id, accessToken: token, limit: 1);
            if (tracks.Total < 1) return null;
            return tracks.Items[0].Track.ExternalUrls.Spotify;
        }

        private Models.Playlist MapToPlaylist(PlaylistSimplified playlistSimplified)
        {
            var playlist = new Models.Playlist
            {
                Id = playlistSimplified.Id,
                Name = playlistSimplified.Name,
                Uri = playlistSimplified.Uri,
                Href = playlistSimplified.Href,
                ExternalUrls = new Models.ExternalUrls { Spotify = playlistSimplified.ExternalUrls.Spotify }
            };

            if (playlistSimplified.Images.Any())
            {
                playlist.Images = playlistSimplified.Images.Select(i => new Models.Image
                {
                    Height = i.Height,
                    Url = i.Url,
                    Width = i.Width
                }).ToArray();
            }

            return playlist;
        }

        public async Task<bool> JoinPlaylist(
            string query,
            string token,
            Station station,
            string stationToken,
            CancellationToken cancellationToken)
        {
            // is the station playing?
            var info = await GetUserNowPlaying(stationToken);

            if (info == null || SpotifyUriHelper.PlaylistId(info.Context.Uri) != SpotifyUriHelper.PlaylistId(station.Playlist.Uri))
            {
                _logger.LogDebug($"JoinPlaylist: station.Playlist.Uri = {station.Playlist.Uri}");
                _logger.LogDebug($"JoinPlaylist: info = {JsonConvert.SerializeObject(info)}");

                return false;
            }

            // TODO: Christian algorithm

            // play from offset
            await RetryHelper.RetryAsync(
                () => _player.PlayPlaylistOffset(
                    info.Context.Uri,
                    info.Item.Id,
                    accessToken: token, positionMs:
                    info.ProgressMs),
                logger: _logger,
                cancellationToken: cancellationToken);

            return true;
        }

        public async Task<CurrentPlaybackContext> GetUserNowPlaying(string token)
        {
            CurrentPlaybackContext info = await RetryHelper.RetryAsync(
                () => _player.GetCurrentPlaybackInfo(token),
                logger: _logger);

            if (info == null || !info.IsPlaying || !new[] { "playlist" }.Contains(info.Context.Type))
            {
                return null;
            }

            return info;
        }

        public async Task<ChannelUser> CreateChannelUserIfNotExists(string channelId, string userId, string username)
        {
            return await _userData.CreateChannelUserIfNotExists(channelId, userId, username);
        }

        public async Task<Station> GetUserStation(ConversationInfo info, string username)
        {
            var stationUri = await _stationData.GetStationUri(RingoBotHelper.ToUserStationUri(info, username));
            if (stationUri == null) return null;
            return await _userData.GetStation(stationUri.ChannelUserId, stationUri.StationId);
        }

        public async Task<Station> GetChannelStation(ConversationInfo info, string conversationName = null)
        {
            if (!info.IsGroup && string.IsNullOrEmpty(conversationName))
            {
                throw new ArgumentException("Must provide conversationName if not in group chat", nameof(conversationName));
            }

            var stationUri = await _stationData.GetStationUri(RingoBotHelper.ToChannelStationUri(info, conversationName));
            if (stationUri == null) return null;
            return await _userData.GetStation(stationUri.ChannelUserId, stationUri.StationId);
        }

        private async Task<Station> CreateStation(
            string channelUserId,
            Models.Playlist playlist,
            string uri,
            string hashtag)
        {
            // save station
            var station = await _userData.CreateStation(channelUserId, playlist, hashtag);

            await _stationData.CreateStationUri(
                station.Id,
                channelUserId,
                uri,
                hashtag);

            _logger.LogInformation($"CreateStation: uri = {uri}, hashtag = {hashtag}, playlist = {playlist.Name}");

            return station;
        }
    }
}
