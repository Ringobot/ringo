using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Data;
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
        public const string RingoBotStatePrefix = "";
        public static readonly Regex RingoBotStateRegex = new Regex($"^{RingoBotStatePrefix}[a-f0-9]{{32}}$");
        private static readonly Regex NonWordRegex = new Regex("\\W");
        private static readonly Regex SpotifyPlaylistUrlRegex = new Regex("playlist\\/[a-zA-Z0-9]+");

        private readonly HttpClient _http;
        private readonly IPlaylistsApi _playlists;
        private readonly IPlayerApi _player;
        private readonly IUserAccountsService _userAccounts;
        private readonly IConfiguration _config;
        private readonly IChannelUserData _userData;
        private readonly IUserStateData _userStateData;
        //private readonly IStationHashcodeData _stationHashcodeData;
        private readonly ILogger _logger;

        public RingoService(
            HttpClient httpClient,
            IPlaylistsApi playlists,
            IPlayerApi player,
            IUserAccountsService userAccounts,
            IConfiguration configuration,
            IChannelUserData channelUserData,
            IUserStateData userStateData,
            ILogger<RingoService> logger
            )
        {
            _http = httpClient;
            _playlists = playlists;
            _player = player;
            _userAccounts = userAccounts;
            _config = configuration;
            _userData = channelUserData;
            _userStateData = userStateData;
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
            CancellationToken cancellationToken)
        {
            // user mentioned?
            Mention mention = null;

            if (turnContext.Activity.Entities != null)
            {
                var mentions = turnContext.Activity.Entities.Where(e => e.Type == "mention").Select(e=> e.GetAs<Mention>());
                mention = mentions.Where(m => m.Mentioned.Id != turnContext.Activity.Recipient.Id).FirstOrDefault();
            }

            if (mention == null || mention.Mentioned == null)
            {
                await turnContext.SendActivityAsync(
                    $"I did not understand 🤔 Try mentioning another user, e.g. `\"{RingoHandleIfGroupChat(turnContext)}join @username\"`",
                    cancellationToken: cancellationToken);
                return;
            }

            //is there a token for the playing user?
            Models.BearerAccessToken mentionedToken = await _userData.GetUserAccessToken(
                turnContext.Activity.ChannelId,
                mention.Mentioned.Id);

            if (mentionedToken == null)
            {
                await turnContext.SendActivityAsync(
                    $"Join failed. @{mention.Mentioned.Name} has not asked Ringo to play anything 🤨 Type `\"{RingoHandleIfGroupChat(turnContext)}play (playlist name)\"` to get started.",
                    cancellationToken: cancellationToken);
                return;
            }

            try
            {
                // is the playing user playing anything?
                var info = await _player.GetCurrentPlaybackInfo(mentionedToken.AccessToken);

                if (info == null || !info.IsPlaying)
                {
                    await turnContext.SendActivityAsync(
                        $"Join failed. @{mention.Mentioned.Name} is no longer playing anything. Type `\"{RingoHandleIfGroupChat(turnContext)}play (playlist name)\"` to get started.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // is the playing user playing a playlist?
                if (info.Context.Type != "playlist")
                {
                    await turnContext.SendActivityAsync(
                        $"Join failed. @{mention.Mentioned.Name} is no longer playing a Playlist.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // play from offset
                await _player.PlayPlaylistOffset(info.Context.Uri, info.Item.Id, accessToken: token, positionMs: info.ProgressMs);

                // TODO: info.Item.Name is current Item name, not Playlist name
                await turnContext.SendActivityAsync(
                    $"@{turnContext.Activity.From.Name} has joined @{mention.Mentioned.Name} playing \"{info.Item.Name}\"! Tell your friends to type `\"{RingoHandleIfGroupChat(turnContext)}join @{turnContext.Activity.From.Name}\"` into Ringobot to join the party! 🎉",
                    cancellationToken: cancellationToken);
            }
            catch (SpotifyApiErrorException ex)
            {
                await turnContext.SendActivityAsync(ex.Message, cancellationToken: cancellationToken);
                return;
            }
        }

        public async Task<TokenResponse> Authorize(
            ITurnContext turnContext,
            CancellationToken cancellationToken)
        {
            Models.BearerAccessToken token = await _userData.GetUserAccessToken(
                turnContext.Activity.ChannelId,
                turnContext.Activity.From.Id);

            // token had not expired and has been validated - return the token
            if (
                token != null
                && !token.AccessTokenExpired
                && token.Validated)
            {
                return MapToTokenResponse(token);
            }

            // token has been validated, but has expired - refresh the token and return it
            if (
                token != null
                && token.AccessTokenExpired
                && token.Validated)
            {
                _logger.LogInformation($"Refreshing access token for channelUserId {ChannelUserId(turnContext)}");
                var bearer = await _userAccounts.RefreshUserAccessToken(token.RefreshToken);

                // map to BearerAccessToken
                token.AccessToken = bearer.AccessToken;
                token.Expires = bearer.Expires;

                if (token.Scope != bearer.Scope)
                {
                    token.Scope = bearer.Scope;
                    _logger.LogWarning($"token Scope has changed when being refreshed. channelID = {turnContext.Activity.ChannelId}, userId = {turnContext.Activity.From.Id}");
                }

                // save token
                await _userData.SaveUserAccessToken(ChannelUserId(turnContext), token);

                return MapToTokenResponse(token);
            }

            _logger.LogInformation($"Requesting Spotify Authorization for channelUserId {ChannelUserId(turnContext)}");

            await CreateChannelUserIfNotExists(
                turnContext.Activity.ChannelId,
                turnContext.Activity.From.Id,
                turnContext.Activity.From.Name);

            // create state token
            string state = $"{RingoBotStatePrefix}{Guid.NewGuid().ToString("N")}".ToLower();

            // validate state token
            if (!RingoBotStateRegex.IsMatch(state))
                throw new InvalidOperationException("Generated state token does not match RingoBotStateRegex");

            // save state token
            await _userStateData.SaveUserStateToken(turnContext.Activity.ChannelId, turnContext.Activity.From.Id, state);

            // get URL
            string url = UserAccountsService.AuthorizeUrl(
                state,
                new[] { "user-read-playback-state", "user-modify-playback-state" },
                _config["SpotifyApiClientId"],
                _config["SpotifyAuthRedirectUri"]);

            var message = MessageFactory.Attachment(
                new Attachment
                {
                    ContentType = HeroCard.ContentType,
                    Content = new HeroCard
                    {
                        Text = "Authorize Ringo bot to use your Spotify account",
                        Buttons = new[]
                    {
                        new CardAction
                        {
                            Title = "Authorize",
                            Text = "Click to Authorize. (Opens in your browser)",
                            Value = url,
                            Type = ActionTypes.OpenUrl,
                        },
                    },
                    },
                },
                text: "To play music, Ringo needs to be authorized to use your Spotify Account.");

            await turnContext.SendActivityAsync(message, cancellationToken);
            return null;
        }

        public async Task<ChannelUser> CreateChannelUserIfNotExists(string channelId, string userId, string username)
        {
            return await _userData.CreateChannelUserIfNotExists(channelId, userId, username);
        }

        public async Task<TokenResponse> ValidateMagicNumber(ITurnContext turnContext, string text, CancellationToken cancellationToken)
        {
            string channelUserId = await _userStateData.GetChannelUserIdFromStateToken(text);
            if (channelUserId == ChannelUser.EncodeId(turnContext.Activity.ChannelId, turnContext.Activity.From.Id))
            {
                await turnContext.SendActivityAsync(
                    $"Magic Number OK. Ringo is authorized to play Spotify. Ready to rock! 😎",
                    cancellationToken: cancellationToken);
                await _userData.SetTokenValidated(turnContext.Activity.ChannelId, turnContext.Activity.From.Id);
                return MapToTokenResponse(await _userData.GetUserAccessToken(
                    turnContext.Activity.ChannelId,
                    turnContext.Activity.From.Id));
            }

            _logger.LogWarning($"Invalid Magic Number \"{text}\" for channelUserId {ChannelUserId(turnContext)}");
            await turnContext.SendActivityAsync(
                $"Magic Number is invalid or has expired. Please try again 🤔",
                cancellationToken: cancellationToken);
            return null;
        }

        private static string RingoHandleIfGroupChat(ITurnContext turnContext) 
            => (RingoBot3.IsGroup(turnContext) ? "@ringo " : string.Empty);

        private static string ChannelUserId(ITurnContext context)
            => ChannelUser.EncodeId(context.Activity.ChannelId, context.Activity.From.Id);

        private static DateTime ToDateTimeFromIso8601(string iso8601)
            => DateTime.Parse(iso8601, null, System.Globalization.DateTimeStyles.RoundtripKind);

        private static string ToIso8601(DateTime? dateTime)
            => dateTime.HasValue ? dateTime.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture) : null;

        private static TokenResponse MapToTokenResponse(Models.BearerAccessToken token)
            => new TokenResponse(
                    connectionName: null,
                    token: token.AccessToken,
                    expiration: ToIso8601(token.Expires));
    }
}
