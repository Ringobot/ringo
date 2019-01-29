using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Ringo.Bot.Net.State;
using SpotifyApi.NetCore;

namespace Ringo.Bot.Net.Services
{
    public class RingoService : IRingoService
    {
        public const string RingoBotStatePrefix = "ringo01";
        public static readonly Regex RingoBotStateRegex = new Regex($"^{RingoBotStatePrefix}[a-f0-9]{{32}}$");

        private readonly HttpClient _http;
        private readonly ISearchApi _search;
        private readonly IPlayerApi _player;
        private readonly IUserAccountsService _userAccounts;
        private readonly IConfiguration _config;

        public RingoService(
            HttpClient httpClient,
            ISearchApi search,
            IPlayerApi player,
            //IUserAccountsService userAccounts,
            IConfiguration configuration)
        {
            _http = httpClient;
            _search = search;
            _player = player;
            //_userAccounts = userAccounts;
            _config = configuration;
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
            string userName,
            ConversationData conversationData,
            CancellationToken cancellationToken)
        {
            Dictionary<string, TokenResponse> userTokens = conversationData.ConversationUserTokens;

            if (
                userTokens.ContainsKey(userName)
                && !string.IsNullOrEmpty(userTokens[userName].Expiration)
                && ToDateTimeFromIso8601(userTokens[userName].Expiration) > DateTime.UtcNow)
            {
                return userTokens[userName];
            }

            // create state token
            string state = $"{RingoBotStatePrefix}{Guid.NewGuid().ToString("N")}";

            // validate state token
            if (!RingoBotStateRegex.IsMatch(state)) throw new InvalidOperationException("Generated state token does not match RingoBotStateRegex");

            // save state token
            conversationData.UserStateTokens[state] = userName;

            // get URL
            string url = UserAccountsService.AuthorizeUrl(
                state,
                new[] { "user-read-playback-state", "user-modify-playback-state" },
                _config["SpotifyApiClientId"],
                _config["SpotifyAuthRedirectUri"]);

            var message = turnContext.Activity;

            if (message.Attachments == null)
            {
                message.Attachments = new List<Attachment>();
            }

            message.Attachments.Add(new Attachment
            {
                ContentType = SigninCard.ContentType,
                Content = new SigninCard
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
            });

            await turnContext.SendActivityAsync(message, cancellationToken);
            return null;
        }

        private static DateTime ToDateTimeFromIso8601(string iso8601)
            => DateTime.Parse(iso8601, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
}
