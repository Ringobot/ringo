using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Data;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public class RingoService : IRingoService
    {
        private static readonly Regex SpotifyPlaylistUrlRegex = new Regex("playlist\\/[a-zA-Z0-9]+");

        private readonly HttpClient _http;
        private readonly IPlaylistsApi _playlists;
        private readonly IPlayerApi _player;
        private readonly IConfiguration _config;
        private readonly IChannelUserData _userData;
        //private readonly IStationHashcodeData _stationHashcodeData;
        private readonly ILogger _logger;
        private IStationData _stationData;

        public RingoService(
            HttpClient httpClient,
            IPlaylistsApi playlists,
            IPlayerApi player,
            IConfiguration configuration,
            IChannelUserData channelUserData,
            ILogger<RingoService> logger
            )
        {
            _http = httpClient;
            _playlists = playlists;
            _player = player;
            _config = configuration;
            _userData = channelUserData;
            _logger = logger;
        }

        public async Task<Models.Playlist> PlayPlaylist(
            ITurnContext turnContext,
            string searchText,
            string accessToken,
            CancellationToken cancellationToken)
        {
            //TODO Model.Playlist
            Models.Playlist playlist = null;

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
                var results = await _playlists.SearchPlaylists(
                    searchText,
                    accessToken: accessToken);

                if (results.Total > 0)
                {
                    playlist = MapToPlaylist(results.Items[0]);

                    await turnContext.SendActivityAsync(
                        $"{results.Total.ToString("N0")} playlists found.",
                        cancellationToken: cancellationToken);
                }
            }
            else
            {
                var playlistSimple = await _playlists.GetPlaylist(uriOrId);
                if (playlistSimple != null) playlist = MapToPlaylist(playlistSimple);
            }

            if (playlist == null)
            {
                await turnContext.SendActivityAsync($"No playlists found!", cancellationToken: cancellationToken);
                return null;
            }

            try
            {
                await _player.PlayPlaylist(playlist.Id, accessToken);
            }
            catch (SpotifyApiErrorException ex)
            {
                _logger.LogError(ex.Message);
                await turnContext.SendActivityAsync($"{ex.Message} 🤔 Try opening Spotify on your device and playing a track for a few seconds. Then try typing `play {searchText}` again", cancellationToken: cancellationToken);
                return null;
            }

            return playlist;
        }

        public async Task<Station> CreateStation(
            ITurnContext turnContext,
            Models.Playlist playlist,
            CancellationToken cancellationToken,
            string hashtag = null)
        {
            // save station
            var station = await _userData.CreateStation(
                RingoBotHelper.ChannelUserId(turnContext),
                playlist, hashtag);

            string channelUserId = RingoBotHelper.ChannelUserId(turnContext);
            ConversationInfo info = RingoBotHelper.NormalizedConversationInfo(turnContext);

            //channelId.lower() / team_id / @username.lower()
            //slack/TA0VBN61L/@daniel
            await _stationData.SaveStationUri(station.Id, channelUserId, RingoBotHelper.ToUserStationUri(info, info.FromName));

            //channelId.lowr() / team_id /#conversation.name.replace(\W).lower()/SlackMessage.event.channel
            //slack/TA0VBN61L/#testing3/CFX3U3TCJ
            await _stationData.SaveStationUri(station.Id, channelUserId,
                RingoBotHelper.ToConversationStationUri(info),
                RingoBotHelper.ToHashtag(info.ConversationName));

            //channelId.lower() / team_id /#playlist_name.replace(\W).lower()
            //slack/TA0VBN61L/#heatwave2019
            await _stationData.SaveStationUri(station.Id, channelUserId,
                RingoBotHelper.ToHashtagStationUri(info, playlist.Name),
                RingoBotHelper.ToHashtag(playlist.Name));

            if (!string.IsNullOrEmpty(hashtag))
            {
                //channelId.lower() / team_id /#hashtag.lower()
                //slack/TA0VBN61L/#heatwave
                await _stationData.SaveStationUri(station.Id, channelUserId,
                    RingoBotHelper.ToHashtagStationUri(info, hashtag),
                    RingoBotHelper.ToHashtag(hashtag));
            }

            return station;
        }

        private Models.Playlist MapToPlaylist(PlaylistSimplified playlistSimplified) => new Models.Playlist
        {
            Id = playlistSimplified.Id,
            Name = playlistSimplified.Name,
            Uri = playlistSimplified.Uri
        };

        public async Task JoinPlaylist(
            ITurnContext turnContext,
            string query,
            string token,
            Station station,
            string stationToken,
            CancellationToken cancellationToken)
        {
            try
            {
                // is the station playing?
                var info = await _player.GetCurrentPlaybackInfo(stationToken);

                if (info == null || !info.IsPlaying || info.Context.Type != "playlist" || info.Context.Uri != station.Playlist.Uri)
                {
                    //TODO: Play button
                    await turnContext.SendActivityAsync(
                        $"Station #{station.Hashtag} is no longer playing. Would you like to Play \"{station.Playlist.Name}\"? Type `\"{RingoHandleIfGroupChat(turnContext)}play {station.Playlist.Uri}\"` to start.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // TODO: Christian algorithm

                // play from offset
                await _player.PlayPlaylistOffset(info.Context.Uri, info.Item.Id, accessToken: token, positionMs: info.ProgressMs);

                await turnContext.SendActivityAsync(
                    $"@{turnContext.Activity.From.Name} has joined #{station.Hashtag}! 🎉",
                    cancellationToken: cancellationToken);
            }
            catch (SpotifyApiErrorException ex)
            {
                await turnContext.SendActivityAsync(ex.Message, cancellationToken: cancellationToken);
                return;
            }
        }

        public async Task<ChannelUser> CreateChannelUserIfNotExists(string channelId, string userId, string username)
        {
            return await _userData.CreateChannelUserIfNotExists(channelId, userId, username);
        }

        internal static string RingoHandleIfGroupChat(ITurnContext turnContext) 
            => (BotHelper.IsGroup(turnContext) ? "@ringo " : string.Empty);

        public async Task<Station> FindStation(ITurnContext turnContext, string query, CancellationToken cancellationToken)
        {
            string uri = null;
            ConversationInfo info = RingoBotHelper.NormalizedConversationInfo(turnContext);

            if (query.StartsWith('@'))
            {
                uri = RingoBotHelper.ToUserStationUri(info, query.Substring(1));
            }
            else if (query.StartsWith('#'))
            {
                uri = RingoBotHelper.ToHashtagStationUri(info, query.Substring(1));
            }
            else
            {
                uri = RingoBotHelper.ToConversationStationUri(info);
            }

            StationUri stationUri = await _stationData.GetStationUri(uri);
            if (stationUri == null) return null;

            return await _userData.GetStation(stationUri.ChannelUserId, stationUri.StationId);
        }
    }
}
