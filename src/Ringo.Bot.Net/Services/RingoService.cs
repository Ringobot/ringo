using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using RingoBotNet.Data;
using RingoBotNet.Models;
using RingoBotNet.State;
using SpotifyApi.NetCore;
using System;
using System.Collections.Generic;
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

        private readonly HttpClient _http;
        private readonly ISearchApi _search;
        private readonly IPlayerApi _player;
        private readonly IUserAccountsService _userAccounts;
        private readonly IConfiguration _config;
        private readonly IChannelUserData _userData;
        private readonly IUserStateData _userStateData;

        public RingoService(
            HttpClient httpClient,
            ISearchApi search,
            IPlayerApi player,
            //IUserAccountsService userAccounts,
            IConfiguration configuration,
            IChannelUserData channelUserData,
            IUserStateData userStateData)
        {
            _http = httpClient;
            _search = search;
            _player = player;
            //_userAccounts = userAccounts;
            _config = configuration;
            _userData = channelUserData;
            _userStateData = userStateData;
        }

        public async Task PlayPlaylist(
            ITurnContext turnContext,
            string searchText,
            string accessToken,
            CancellationToken cancellationToken)
        {
            // search for the Playlist
            var results = await _search.Search(
                searchText,
                SpotifySearchTypes.Playlist,
                accessToken: accessToken);

            // if none found, return
            if (results.Playlists.Total == 0)
            {
                await turnContext.SendActivityAsync($"No playlists found!", cancellationToken: cancellationToken);
                return;
            }

            // Play the first playlist found
            await turnContext.SendActivityAsync(
                $"{results.Playlists.Total.ToString("N0")} playlists found.",
                cancellationToken: cancellationToken);

            try
            {
                await _player.PlayPlaylist(results.Playlists.Items[0].Id, accessToken);
                await turnContext.SendActivityAsync(
                    $"{turnContext.Activity.From.Name} is playing \"{results.Playlists.Items[0].Name}\"",
                    cancellationToken: cancellationToken);
            }
            catch (SpotifyApiErrorException ex)
            {
                await turnContext.SendActivityAsync(ex.Message, cancellationToken: cancellationToken);
                return;
            }
        }

        public async Task JoinPlaylist(
            ITurnContext turnContext,
            string joinUsername,
            ConversationData conversationData,
            string token,
            CancellationToken cancellationToken)
        {
            // is there a token for the playing user?
            if (!conversationData.ConversationUserTokens.ContainsKey(joinUsername))
            {
                await turnContext.SendActivityAsync(
                    $"Join failed. {joinUsername} has not asked Ringo to play anything.",
                    cancellationToken: cancellationToken);
                return;
            }

            var playingUsertoken = conversationData.ConversationUserTokens[joinUsername];

            try
            {
                // is the playing user playing anything?
                var info = await _player.GetCurrentPlaybackInfo(playingUsertoken.Token);

                if (!info.IsPlaying)
                {
                    await turnContext.SendActivityAsync(
                        $"Join failed. {joinUsername} is no longer playing anything.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // is the playing user playing a playlist?
                if (info.Context.Type != "playlist")
                {
                    await turnContext.SendActivityAsync(
                        $"Join failed. {joinUsername} is no longer playing a Playlist.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // play from offset
                await _player.PlayPlaylistOffset(info.Context.Uri, info.Item.Id, accessToken: token, positionMs: info.ProgressMs);

                await turnContext.SendActivityAsync(
                    $"{turnContext.Activity.From.Name} has joined {joinUsername} playing \"{info.Item.Name}\"",
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

            if (
                token != null
                && token.Expires.HasValue
                && token.Expires > DateTime.UtcNow
                && token.Validated)
            {
                return new TokenResponse(
                    connectionName: null,
                    token: token.AccessToken,
                    expiration: ToIso8601(DateTime.UtcNow));
            }

            // create state token
            string state = $"{RingoBotStatePrefix}{Guid.NewGuid().ToString("N")}".ToLower();

            // validate state token
            if (!RingoBotStateRegex.IsMatch(state)) throw new InvalidOperationException("Generated state token does not match RingoBotStateRegex");

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

        public async Task CreateChannelUserIfNotExists(string channelId, string userId, string username)
        {
            await _userData.CreateChannelUserIfNotExists(channelId, userId, username);
        }

        public async Task ValidateMagicNumber(ITurnContext turnContext, string text, CancellationToken cancellationToken)
        {
            string channelUserId = await _userStateData.GetChannelUserIdFromStateToken(text);
            if (channelUserId == ChannelUser.EncodeId(turnContext.Activity.ChannelId, turnContext.Activity.From.Id))
            {
                await turnContext.SendActivityAsync(
                    $"Magic Number OK. Ringo is authorized to play Spotify. Ready to rock! 😎",
                    cancellationToken: cancellationToken);
                await _userData.SetTokenValidated(turnContext.Activity.ChannelId, turnContext.Activity.From.Id);
            }
            else
            {
                await turnContext.SendActivityAsync(
                    $"Magic Number is invalid or has expired. Please try again 🤔",
                    cancellationToken: cancellationToken);
            }
        }

        private static DateTime ToDateTimeFromIso8601(string iso8601)
            => DateTime.Parse(iso8601, null, System.Globalization.DateTimeStyles.RoundtripKind);

        private static string ToIso8601(DateTime dateTime)
            => dateTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
    }
}
