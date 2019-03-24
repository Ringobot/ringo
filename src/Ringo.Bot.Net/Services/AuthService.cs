using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RingoBotNet.Data;
using RingoBotNet.Helpers;
using RingoBotNet.Models;
using SpotifyApi.NetCore;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public class AuthService : IAuthService
    {
        public const string RingoBotStatePrefix = "";
        public static readonly Regex RingoBotStateRegex = new Regex($"^{RingoBotStatePrefix}[a-f0-9]{{32}}$");

        private readonly HttpClient _http;
        private readonly IUserAccountsService _userAccounts;
        private readonly IConfiguration _config;
        private readonly IUserStateData _userStateData;
        private readonly IChannelUserData _userData;
        private readonly ILogger _logger;


        public AuthService(
            HttpClient httpClient,
            IUserAccountsService userAccounts,
            IConfiguration configuration,
            IChannelUserData channelUserData,
            IUserStateData userStateData,
            ILogger<RingoService> logger)
        {
            _http = httpClient;
            _userAccounts = userAccounts;
            _config = configuration;
            _userData = channelUserData;
            _userStateData = userStateData;
            _logger = logger;
        }

        public async Task<TokenResponse> Authorize(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var info = RingoBotHelper.NormalizedConversationInfo(turnContext);

            TokenResponse token = await GetAccessToken(turnContext.Activity.ChannelId, turnContext.Activity.From.Id);
            if (token != null) return token;

            // User is not authorized by Spotify
            if (BotHelper.IsGroup(turnContext))
            {
                // Don't start authorisation dance in Group chat
                await turnContext.SendActivityAsync(
                    $"Before you play or join with Ringo you need to authorize Spotify. DM (direct message) the word `\"{RingoBotCommands.AuthCommand[0]}\"` to @{info.BotName} to continue.",
                    cancellationToken: cancellationToken);
                return null;
            }

            _logger.LogInformation($"Requesting Spotify Authorization for channelUserId {RingoBotHelper.ChannelUserId(turnContext)}");

            await _userData.CreateChannelUserIfNotExists(
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

        public async Task<TokenResponse> ValidateMagicNumber(
            ITurnContext turnContext,
            string text,
            CancellationToken cancellationToken)
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

            _logger.LogWarning($"Invalid Magic Number \"{text}\" for channelUserId {RingoBotHelper.ChannelUserId(turnContext)}");
            await turnContext.SendActivityAsync(
                $"Magic Number is invalid or has expired. Please try again 🤔",
                cancellationToken: cancellationToken);
            return null;
        }

        public async Task ResetAuthorization(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await _userData.ResetAuthorization(RingoBotHelper.ChannelUserId(turnContext), cancellationToken);
        }

        /// <summary>
        /// Gets a current Bearer Token for the Spotify service, refreshing if neccessary
        /// </summary>
        public async Task<TokenResponse> GetAccessToken(string channelId, string userId)
            => await GetAccessToken(RingoBotHelper.ChannelUserId(channelId, userId));

        public async Task<TokenResponse> GetAccessToken(string channelUserId)
        {
            Models.BearerAccessToken token = await _userData.GetUserAccessToken(channelUserId);

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
                _logger.LogInformation($"Refreshing access token for channelUserId {channelUserId}");
                var bearer = await _userAccounts.RefreshUserAccessToken(token.RefreshToken);

                // map to BearerAccessToken
                token.AccessToken = bearer.AccessToken;
                token.Expires = bearer.Expires;

                if (token.Scope != bearer.Scope)
                {
                    token.Scope = bearer.Scope;
                    _logger.LogWarning($"token Scope has changed when being refreshed. channeUserlID = {channelUserId}");
                }

                // save token
                await _userData.SaveUserAccessToken(channelUserId, token);

                return MapToTokenResponse(token);
            }

            return null;
        }

        private static TokenResponse MapToTokenResponse(Models.BearerAccessToken token)
            => new TokenResponse(
                connectionName: null,
                token: token.AccessToken,
                expiration: ToIso8601(token.Expires));

        // Todo: DateHelper
        private static DateTime ToDateTimeFromIso8601(string iso8601)
            => DateTime.Parse(iso8601, null, System.Globalization.DateTimeStyles.RoundtripKind);

        private static string ToIso8601(DateTime? dateTime)
            => dateTime.HasValue ? dateTime.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture) : null;
    }
}
