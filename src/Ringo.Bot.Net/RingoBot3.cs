using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RingoBotNet.Services;
using RingoBotNet.State;

namespace RingoBotNet
{
    public class RingoBot3 : IBot
    {
        // feature flags
        private const bool USE_BOT_BUILDER_AUTH = false;

        private const string ConnectionName = "spotify_connection_3";
        private static readonly Regex _magicCodeRegex = new Regex("^[0-9]{6}$");
        private readonly ILogger _logger;
        private readonly IRingoService _ringoService;
        private readonly RingoBotAccessors _stateAccessors;
        private readonly string _contentRoot;

        public RingoBot3(
            ILogger<RingoBot3> logger, 
            IRingoService ringoService,
            RingoBotAccessors accessors,
            IConfiguration configuration)
        {
            _logger = logger;
            _ringoService = ringoService;
            _stateAccessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
            _contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            //// Get State
            var userProfile = await _stateAccessors.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
            var conversationData = await _stateAccessors.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    if (NotListening(turnContext))
                    {
                        _logger.LogDebug("Not listening. In a group chat and not mentioned.");
                        break;
                    }

                    _logger.LogDebug($"{turnContext.Activity.Type}:{turnContext.Activity.Text}");
                    _logger.LogDebug($"\"Activity\":{JsonConvert.SerializeObject(turnContext.Activity)}");

                    if (string.IsNullOrEmpty(turnContext.Activity.Text))
                    {
                        _logger.LogDebug("turnContext.Activity.Text is null or empty. Doing nothing.");
                        break;
                    }

                    string text = turnContext.Activity.Text.Trim();
                    // remove the mention
                    if (IsGroup(turnContext) && IsMentioned(turnContext))
                    {
                        text = text.Replace($"@{turnContext.Activity.Recipient.Name} ", string.Empty);
                    }

                    string command = text.Split(' ')[0].ToLower();
                    string query = text.Remove(0, command.Length).Trim();

                    _logger.LogDebug($"Parsed \"{turnContext.Activity.Text}\" as text = \"{text}\", command = \"{command}\", query = \"{query}\"");

                    // login
                    if (USE_BOT_BUILDER_AUTH && command == "login")
                    {
                        var adapter = (BotFrameworkAdapter)turnContext.Adapter;
                        var token2 = await adapter.GetUserTokenAsync(turnContext, ConnectionName, null, cancellationToken);

                        if (token2 == null)
                        {
                            await turnContext.SendActivityAsync("Logging you in...");
                            await SendOAuthCardAsync(turnContext, cancellationToken);
                        }
                        else
                        {
                            _logger.LogDebug($"Token = {token2.Token.Substring(0, 5)}...");
                            await turnContext.SendActivityAsync($"Already logged in with Token {token2.Token.Substring(0, 5)} 👌. Type `logout` if you would like to logout.");
                        }

                        break;
                    }

                    if (USE_BOT_BUILDER_AUTH && command == "logout")
                    {
                        var adapter = (BotFrameworkAdapter)turnContext.Adapter;
                        await adapter.SignOutUserAsync(turnContext, ConnectionName, cancellationToken: cancellationToken);
                        await turnContext.SendActivityAsync("You have been logged out.");
                        break;
                    }

                    if (command == "play")
                    {
                        TokenResponse token2;

                        if (USE_BOT_BUILDER_AUTH)
                        {
                            var adapter = (BotFrameworkAdapter)turnContext.Adapter;
                            token2 = await adapter.GetUserTokenAsync(turnContext, ConnectionName, null, cancellationToken);

                            if (token2 == null)
                            {
                                await turnContext.SendActivityAsync("Can't play because I can't find a token. Try `\"login\"`.");
                                break;
                            }
                        }
                        else
                        {
                            token2 = await _ringoService.Authorize(turnContext, cancellationToken);

                            if (token2 == null)
                            {
                                // resume after auth
                                userProfile.ResumeAfterAuthorizationWith = (command, query);
                                break; 
                            }
                        }

                        //conversationData.ConversationUserTokens[FromUserName(turnContext)] = token2;

                        await _ringoService.PlayPlaylist(
                            turnContext,
                            query,
                            token2.Token,
                            cancellationToken);

                        break;
                    }

                    if (command == "join" && !USE_BOT_BUILDER_AUTH)
                    {
                        var token2 = await _ringoService.Authorize(turnContext, cancellationToken);

                        if (token2 == null)
                        {
                            // resume after auth
                            userProfile.ResumeAfterAuthorizationWith = (command, query);
                            break;
                        }

                        // Join
                        await _ringoService.JoinPlaylist(
                            turnContext,
                            query,
                            token2.Token,
                            cancellationToken);

                        break;
                    }

                    // authorize
                    if (command == "authorize")
                    {
                        if (IsGroup(turnContext))
                        {
                            await turnContext.SendActivityAsync(
                                $"Ringo cannot authorize you in Group Chat. DM (direct message) @ringo instead.", 
                                cancellationToken: cancellationToken);
                            break;
                        }

                        var token2 = await _ringoService.Authorize(turnContext, cancellationToken);

                        if (token2 == null)
                        {
                            // clear resume after auth
                            userProfile.ResumeAfterAuthorizationWith = (null, null);
                            break;
                        }

                        await turnContext.SendActivityAsync(
                                            $"Ringo is authorized to play Spotify. Ready to rock! 😎",
                                            cancellationToken: cancellationToken);
                        break;
                    }

                    // magic number
                    if (!USE_BOT_BUILDER_AUTH && RingoService.RingoBotStateRegex.IsMatch(text))
                    {
                        var token2 = await _ringoService.ValidateMagicNumber(turnContext, text, cancellationToken);

                        //TODO: needs improvement 🤔
                        if (userProfile.ResumeAfterAuthorizationWith.command == "play")
                        {
                            await _ringoService.PlayPlaylist(
                                turnContext,
                                userProfile.ResumeAfterAuthorizationWith.query,
                                token2.Token,
                                cancellationToken);
                        }
                        else if (userProfile.ResumeAfterAuthorizationWith.command == "join")
                        {
                            await _ringoService.JoinPlaylist(
                                turnContext,
                                userProfile.ResumeAfterAuthorizationWith.query,
                                token2.Token,
                                cancellationToken);
                        }

                        break;
                    }

                    if (USE_BOT_BUILDER_AUTH && _magicCodeRegex.IsMatch(text))
                    {
                        var adapter = (BotFrameworkAdapter)turnContext.Adapter;
                        await turnContext.SendActivityAsync("Checking your magic number...");
                        var token2 = await adapter.GetUserTokenAsync(turnContext, ConnectionName, text, cancellationToken);

                        if (token2 == null)
                        {
                            await turnContext.SendActivityAsync("Could not log you in 😢");
                        }
                        else
                        {
                            Trace.WriteLine($"Token = {token2.Token.Substring(0, 5)}...", "RingoBot3");
                            await turnContext.SendActivityAsync($"Logged in with Token {token2.Token.Substring(0, 5)} 👌");
                        }

                        break;
                    }

                    if (command == "help")
                    {
                        
                        await turnContext.SendActivityAsync(
                            await File.ReadAllTextAsync($"{_contentRoot}/Text/help.txt", cancellationToken));
                        break;
                    }

                    if (command == "more")
                    {
                        await turnContext.SendActivityAsync(
                            await File.ReadAllTextAsync($"{_contentRoot}/Text/more_help.txt", cancellationToken));
                        break;
                    }

                    // any other text
                    if (text.Length > 0)
                    {
                        await turnContext.SendActivityAsync($"You wrote \"{turnContext.Activity.Text}\" but I did not understand 🤔 try `\"help\"`.");
                        break;
                    }

                    // do nothing
                    Trace.WriteLine("Doing nothing", "RingoBot3");

                    break;

                case ActivityTypes.ConversationUpdate:
                    if (turnContext.Activity.MembersAdded != null)
                    {
                        await SendWelcomeMessageAsync(turnContext, cancellationToken);
                        await CreateChannelUsers(turnContext, cancellationToken);
                    }

                    break;

                case ActivityTypes.Event:
                case ActivityTypes.Invoke:
                    await turnContext.SendActivityAsync("Checking your token...");
                    var token = await RecognizeTokenAsync(turnContext, cancellationToken);
                    if (token == null)
                    {
                        await turnContext.SendActivityAsync("Could not log you in 😢");
                    }
                    else
                    {
                        Trace.WriteLine($"Token = {token.Token.Substring(0, 5)}...", "RingoBot3");
                        await turnContext.SendActivityAsync($"Logged in with Token {token.Token.Substring(0, 5)} 👌");
                    }

                    break;

                default:
                    Trace.TraceWarning($"{turnContext.Activity.Type} is not handled", "RingoBot3");
                    break;
            }

            //// Commit state
            await _stateAccessors.UserProfileAccessor.SetAsync(turnContext, userProfile);
            await _stateAccessors.UserState.SaveChangesAsync(turnContext);
            await _stateAccessors.ConversationDataAccessor.SetAsync(turnContext, conversationData);
            await _stateAccessors.ConversationState.SaveChangesAsync(turnContext);

        }

        private static bool NotListening(ITurnContext turnContext)
        {
            // if in a group and the bot is not mentioned, ignore this dialog
            return (IsGroup(turnContext) && !IsMentioned(turnContext));
        }

        internal static bool IsGroup(ITurnContext turnContext)
        {
            return turnContext.Activity.Conversation != null 
                && turnContext.Activity.Conversation.IsGroup.HasValue 
                && turnContext.Activity.Conversation.IsGroup.Value;
        }

        private static bool IsMentioned(ITurnContext turnContext)
        {
            if (turnContext.Activity.Entities == null) return false;
            var mentions = turnContext.Activity.Entities.Where(e => e.Type == "mention").Select(e=>e.GetAs<Mention>());
            return mentions.Any(m => m.Mentioned.Id == turnContext.Activity.Recipient.Id);
        }

        private async Task CreateChannelUsers(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await _ringoService.CreateChannelUserIfNotExists(
                        turnContext.Activity.ChannelId,
                        member.Id,
                        member.Name
                        );
                }
            }
        }

        private static string FromUserName(ITurnContext turnContext) => turnContext.Activity.From.Name.ToLower();

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Hi {member.Name}, I'm Ringo! Try `\"play (playlist name)\"` to begin. Or type `\"help\"`.",
                        cancellationToken: cancellationToken);
                }
            }
        }

        private async Task SendOAuthCardAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var message = turnContext.Activity;

            if (message.Attachments == null)
            {
                message.Attachments = new List<Attachment>();
            }

            var adapter = (BotFrameworkAdapter)turnContext.Adapter;
            string link = await adapter.GetOauthSignInLinkAsync(turnContext, ConnectionName, cancellationToken);

            message.Attachments.Add(new Attachment
            {
                ContentType = OAuthCard.ContentType,
                Content = new OAuthCard
                {
                    Text = "Please sign in",
                    ConnectionName = ConnectionName,
                    Buttons = new[]
                    {
                        new CardAction
                        {
                            Title = "Sign In",
                            Text = "Sign In",
                            Type = ActionTypes.Signin,
                        },
                        //new CardAction
                        //{
                        //    Title = "Sign in link",
                        //    Text = "Try this link if Sign-in button does not work. (Opens in your browser)",
                        //    Value = link,
                        //    Type = ActionTypes.OpenUrl,
                        //},
                    },
                },
            });

            await turnContext.SendActivityAsync(message, cancellationToken);
        }

        private async Task<TokenResponse> RecognizeTokenAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var adapter = (BotFrameworkAdapter)turnContext.Adapter;

            if (IsTokenResponseEvent(turnContext))
            {
                // The bot received the token directly
                var tokenResponseObject = turnContext.Activity.Value as JObject;
                var token = tokenResponseObject?.ToObject<TokenResponse>();
                return token;
            }
            else if (IsTeamsVerificationInvoke(turnContext))
            {
                var magicCodeObject = turnContext.Activity.Value as JObject;
                var magicCode = magicCodeObject.GetValue("state")?.ToString();

                var token = await adapter.GetUserTokenAsync(turnContext, ConnectionName, magicCode, cancellationToken);
                return token;
            }

            return null;
        }

        private bool IsTokenResponseEvent(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            return activity.Type == ActivityTypes.Event && activity.Name == "tokens/response";
        }

        private bool IsTeamsVerificationInvoke(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            return activity.Type == ActivityTypes.Invoke && activity.Name == "signin/verifyState";
        }
    }
}
