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
        private static readonly Regex NonWordRegex = new Regex("\\W");
        private static readonly Regex SpotifyPlaylistUrlRegex = new Regex("playlist\\/[a-zA-Z0-9]+");

        private readonly HttpClient _http;
        private readonly IPlaylistsApi _playlists;
        private readonly IPlayerApi _player;
        private readonly IConfiguration _config;
        private readonly IChannelUserData _userData;
        //private readonly IStationHashcodeData _stationHashcodeData;
        private readonly ILogger _logger;

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

        public async Task PlayPlaylist(
            ITurnContext turnContext,
            string searchText,
            string accessToken,
            CancellationToken cancellationToken)
        {
            //TODO Model.Playlist
            (string id, string name) playlist = (null, null);

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

                // if none found, return
                if (results.Total > 0)
                {
                    playlist = (results.Items[0].Id, results.Items[0].Name);

                    await turnContext.SendActivityAsync(
                        $"{results.Total.ToString("N0")} playlists found.",
                        cancellationToken: cancellationToken);
                }
            }
            else
            {
                var playlistSimple = await _playlists.GetPlaylist(uriOrId);
                if (playlistSimple != null) playlist = (playlistSimple.Id, playlistSimple.Name);
            }

            try
            {
                if (playlist.id == null)
                {
                    await turnContext.SendActivityAsync($"No playlists found!", cancellationToken: cancellationToken);
                    return;
                }

                await _player.PlayPlaylist(playlist.id, accessToken);
                await turnContext.SendActivityAsync(
                    $"{turnContext.Activity.From.Name} is playing \"{playlist.name}\"",
                    cancellationToken: cancellationToken);
            }
            catch (SpotifyApiErrorException ex)
            {
                _logger.LogError(ex.Message);
                await turnContext.SendActivityAsync($"{ex.Message} 🤔 Try opening Spotify on your device and playing a track for a few seconds. Then try typing `play {searchText}` again", cancellationToken: cancellationToken);
                return;
            }

            // generate hashcode
            string hashcode = $"#{NonWordRegex.Replace(playlist.name, string.Empty)}";

            // save the hashcode
            //hashcode = _stationHashcodeData.CreateStationHashcode(ChannelUserId(turnContext), hashcode);

            // save station
            //var station = await _userData.CreateStation(
            //    turnContext.Activity.ChannelId,
            //    turnContext.Activity.From.Id,
            //    turnContext.Activity.From.Name,
            //    hashcode,
            //    results.Playlists.Items[0]);

            //TODO: user station.Hashcode
            //await turnContext.SendActivityAsync(
            //    $"Tell your friends to type `join {hashcode}` into Ringobot to join the party! 🥳",
            //    cancellationToken: cancellationToken);

            await turnContext.SendActivityAsync(
                $"Tell your friends to type `\"{RingoHandleIfGroupChat(turnContext)}join @{turnContext.Activity.From.Name}\"` into Ringobot to join the party! 🎉",
                cancellationToken: cancellationToken);
        }

        public async Task JoinPlaylist(
            ITurnContext turnContext,
            string query,
            string token,
            ChannelAccount mentioned,
            string mentionedToken,
            CancellationToken cancellationToken)
        {

            try
            {
                // is the playing user playing anything?
                var info = await _player.GetCurrentPlaybackInfo(mentionedToken);

                if (info == null || !info.IsPlaying)
                {
                    await turnContext.SendActivityAsync(
                        $"@{mentioned.Name} is no longer playing anything. Type `\"{RingoHandleIfGroupChat(turnContext)}play (playlist name)\"` to get started.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // is the playing user playing a playlist?
                if (info.Context.Type != "playlist")
                {
                    await turnContext.SendActivityAsync(
                        $"@{mentioned.Name} is no longer playing a Playlist.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // play from offset
                await _player.PlayPlaylistOffset(info.Context.Uri, info.Item.Id, accessToken: token, positionMs: info.ProgressMs);

                // TODO: info.Item.Name is current Item name, not Playlist name
                await turnContext.SendActivityAsync(
                    $"@{turnContext.Activity.From.Name} has joined @{mentioned.Name} playing \"{info.Item.Name}\"! Tell your friends to type `\"{RingoHandleIfGroupChat(turnContext)}join @{mentioned.Name}\"` into Ringobot to join the party! 🎉",
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
    }
}
