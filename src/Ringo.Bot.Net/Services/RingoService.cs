﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RingoBotNet.Data;
using RingoBotNet.Helpers;
using RingoBotNet.Mappers;
using RingoBotNet.Models;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Helpers;
using SpotifyApi.NetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public class RingoService : IRingoService
    {
        private static readonly string[] SupportedSpotifyItemTypes = new[] { "playlist", "album" };
        private static readonly Regex SpotifyPlaylistUrlRegex = new Regex("playlist\\/[a-zA-Z0-9]+");

        private readonly IPlaylistsApi _playlists;
        private readonly IAlbumsApi _albums;
        private readonly IArtistsApi _artists;
        private readonly IPlayerApi _player;
        private readonly IConfiguration _config;
        private readonly IChannelUserData _userData;
        private readonly ILogger _logger;
        private IStationData _stationData;

        public RingoService(
            IPlaylistsApi playlists,
            IPlayerApi player,
            IAlbumsApi albums,
            IArtistsApi artists,
            IConfiguration configuration,
            IChannelUserData channelUserData,
            IStationData stationData,
            ILogger<RingoService> logger
            )
        {
            _playlists = playlists;
            _albums = albums;
            _artists = artists;
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
                    playlists = results.Items.Take(3).Select(ItemMappers.MapToPlaylist).ToArray();
                }
            }
            else
            {
                playlists = new[] { await GetPlaylist(accessToken, uriOrId) };
            }

            return playlists;
        }

        public async Task<Models.Album> GetAlbum(string token, string uri)
            => ItemMappers.MapToAlbum(await RetryHelper.RetryAsync(() => _albums.GetAlbum(uri, accessToken: token), logger: _logger));

        public async Task<Models.Artist> GetArtist(string token, string uri)
            => ItemMappers.MapToArtist(await RetryHelper.RetryAsync(() => _artists.GetArtist(uri, accessToken: token), logger: _logger));

        public async Task<Models.Playlist> GetPlaylist(string token, string uriOrId)
            => ItemMappers.MapToPlaylist(await RetryHelper.RetryAsync(
                () => _playlists.GetPlaylist(uriOrId, accessToken: token),
                logger: _logger));

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

        public async Task<bool> JoinPlaylist(
            string query,
            string token,
            Station station,
            string stationToken,
            CancellationToken cancellationToken)
        {
            // is the station playing?
            // default the position to what was returned by get info
            var info = await GetUserNowPlaying(stationToken);
            (string itemId, (long positionMs, DateTime atUtc) position) itemPosition = (info.Item.Id, (info.ProgressMs, DateTime.UtcNow));

            if (
                info == null
                || !info.IsPlaying
                || info.Context == null
                || SpotifyUriHelper.NormalizeUri(info.Context.Uri) != SpotifyUriHelper.NormalizeUri(station.SpotifyUri))
            {
                _logger.LogInformation($"JoinPlaylist: No longer playing station {station}", station);
                _logger.LogDebug($"JoinPlaylist: station.Playlist.Uri = {station.Playlist.Uri}");
                _logger.LogDebug($"JoinPlaylist: info = {JsonConvert.SerializeObject(info)}");
                return false;
            }

            if (!SupportedSpotifyItemTypes.Contains(station.SpotifyContextType))
                throw new NotSupportedException($"\"{station.SpotifyContextType}\" is not a supported Spotify context type");

            var offset = await GetOffset(stationToken);

            if (offset.success)
            {
                // reset position to Station position
                itemPosition.itemId = offset.itemId;
                itemPosition.position = offset.position;
            }

            // shuffle and repeat
            if (info.ShuffleState)
            {
                await _player.Shuffle(false, accessToken: token, deviceId: info.Device.Id);
            }

            if (info.RepeatState != RepeatStates.Off)
            {
                await _player.Repeat(RepeatStates.Off, accessToken: token, deviceId: info.Device.Id);
            }

            try
            {
                // mute joining player
                await _player.Volume(0, accessToken: token, deviceId: info.Device.Id);

                // play from offset
                switch (station.SpotifyContextType)
                {
                    case "album":
                        await RetryHelper.RetryAsync(
                            () => _player.PlayAlbumOffset(
                                info.Context.Uri,
                                info.Item.Id,
                                accessToken: token,
                                positionMs: PositionMsNow(itemPosition.position).positionMs),
                            logger: _logger,
                            cancellationToken: cancellationToken);
                        break;

                    case "playlist":
                        await RetryHelper.RetryAsync(
                            () => _player.PlayPlaylistOffset(
                                info.Context.Uri,
                                info.Item.Id,
                                accessToken: token,
                                positionMs: PositionMsNow(itemPosition.position).positionMs),
                            logger: _logger,
                            cancellationToken: cancellationToken);
                        break;
                }

                if (offset.success) await SyncJoiningPlayer(stationToken: stationToken, joiningToken: token);

            }
            finally
            {
                // unmute joining player
                await _player.Volume((int)info.Device.VolumePercent, accessToken: token, deviceId: info.Device.Id);
            }

            return true;
        }

        /// <summary>
        /// Given a positionMs at a point in time in the past, returns the positionMs now (or at a given nowUtc)
        /// </summary>
        private static (long positionMs, DateTime atUtc) PositionMsNow(
            (long positionMs, DateTime atUtc) position, 
            DateTime? nowUtc = null)
        {
            DateTime now = nowUtc ?? DateTime.UtcNow;
            return (position.positionMs + Convert.ToInt64(now.Subtract(position.atUtc).TotalMilliseconds), now);
        }

        /// <summary>
        /// Returns the difference in milliseconds between two (position, dateTime) tuples.
        /// </summary>
        private static long PositionDiff((long positionMs, DateTime atUtc) position1, (long positionMs, DateTime atUtc) position2)
        {
            DateTime epoch1 = position1.atUtc.AddMilliseconds(-position1.positionMs);
            DateTime epoch2 = position2.atUtc.AddMilliseconds(-position2.positionMs);
            return Convert.ToInt64(epoch2.Subtract(epoch1).TotalMilliseconds);
        }

        private async Task SyncJoiningPlayer(string stationToken, string joiningToken)
        {
            // TODO:
            // Christian algorithm
            //  T + RTT/2
            //  Time + RoundTripTime / 2
            var joinerNewPositionMs = new Func<(long, DateTime), (long, DateTime), (long, DateTime)>((
                (long position, DateTime atUtc) stationPosition,
                (long positionMs, DateTime atUtc) joinerPosition) =>
            {
                // error is positive if joiner lags station
                long error = PositionDiff(stationPosition, joinerPosition);

                _logger.LogDebug(
                    $"SyncJoiningPlayer: joinerNewPositionMs: Token = {BotHelper.TokenForLogging(joiningToken)}, error = {error}");

                // shift position of joiner relative to time
                return (joinerPosition.positionMs + error, joinerPosition.atUtc);
            });

            var syncJoiner = new Func<(long, DateTime), Task<(bool, long, (long, DateTime))>>(async ((long positionMs, DateTime atUtc) lastPosition) =>
            {
                (bool success, string itemId, (long positionMs, DateTime currentUtc) position) current = await GetOffset(joiningToken);
                if (!current.success) return (false, 0, (0, DateTime.MinValue));

                (long positionMs, DateTime atUtc) adjustedPosition = joinerNewPositionMs(lastPosition, current.position);
                (long positionMs, DateTime atUtc) newPosition = PositionMsNow(adjustedPosition);
                if (newPosition.positionMs < 0) return (false, 0, (0, DateTime.MinValue));

                // play @ Station_Playhead_Now + error
                await _player.Seek(newPosition.positionMs, accessToken: joiningToken);

                long error = PositionDiff(current.position, newPosition);

                _logger.LogDebug(
                    $"SyncJoiningPlayer: syncJoiner: Token = {BotHelper.TokenForLogging(joiningToken)}, Joiner was synced from {current.position} to {newPosition} (error = {error} ms) based on station position of {lastPosition}");

                return (true, error, newPosition);
            });

            //const long errorAdjustmentThresholdMs = 100;

            var stationOffset = await GetOffset(stationToken);

            (bool success, long error, (long positionMs, DateTime atUtc) newPosition) attempt1 
                = await syncJoiner(stationOffset.position);

            // if attempt as unsuccessful, or the adjusted error was less than errorAdjustmentThresholdMs, return
            //if (!attempt1.success || Math.Abs(attempt1.error) <= errorAdjustmentThresholdMs) return;

            // do it again
            stationOffset = await GetOffset(stationToken);
            await syncJoiner(stationOffset.position);
        }

        protected internal async Task<(bool success, string itemId, (long progressMs, DateTime atUtc) position)> GetOffset(
            string token)
        {
            // Christian algorithm
            //  T + RTT/2
            //  Time + RoundTripTime / 2

            var results = new List<(string itemId, long progressMs, long roundtripMs, DateTime utc)>();

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var rt = await GetRoundTrip(token);
                    results.Add(rt);
                    _logger.LogDebug($"GetOffset: Token = {BotHelper.TokenForLogging(token)}, GetRoundTrip = {rt}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"GetOffset: Try {i + 1} of 3 failed");
                }
            }

            if (results.Any())
            {
                (string itemId, long progressMs, long roundtripMs, DateTime utc) = results.OrderBy(r => r.roundtripMs).First();
                var result = (true, itemId, (progressMs + (roundtripMs / 2), utc));
                _logger.LogDebug($"GetOffset: {BotHelper.TokenForLogging(token)}, result = {result}");
                return result;
            }

            return (false, null, (0, DateTime.MinValue));
        }

        protected internal virtual async Task<(string itemId, long progressMs, long roundtripMs, DateTime utc)> GetRoundTrip(
            string token)
        {
            // DateTime has enough fidelity for these timings
            var start = DateTime.UtcNow;
            CurrentPlaybackContext info1 = await _player.GetCurrentPlaybackInfo(token);
            var finish = DateTime.UtcNow;
            double rtt = finish.Subtract(start).TotalMilliseconds;
            return (info1.Item.Id, info1.ProgressMs, Convert.ToInt64(rtt), finish);
        }

        public async Task<CurrentPlaybackContext> GetUserNowPlaying(string token) 
            => await RetryHelper.RetryAsync(
                () => _player.GetCurrentPlaybackInfo(token),
                logger: _logger);

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
            string uri,
            string hashtag,
            Models.Album album = null,
            Models.Playlist playlist = null)
        {
            // save station
            var station = await _userData.CreateStation(channelUserId, album, playlist, hashtag);

            await _stationData.CreateStationUri(
                station.Id,
                channelUserId,
                uri,
                hashtag);

            _logger.LogInformation($"CreateStation: uri = {uri}, hashtag = {hashtag}, item = {(album?.Name ?? playlist?.Name)}");

            return station;
        }
    }
}
