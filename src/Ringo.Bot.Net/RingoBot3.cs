using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Ringo.Bot.Net.Services;

namespace Ringo.Bot.Net
{
    public class RingoBot3 : IBot
    {
        private const string ConnectionName = "spotify_connection_3";
        private static readonly Regex _magicCodeRegex = new Regex("^[0-9]{6}$");
        private readonly ILogger _logger;
        private readonly IRingoService _ringoService;

        public RingoBot3(ILogger<RingoBot3> logger, IRingoService ringoService)
        {
            _logger = logger;
            _ringoService = ringoService;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Trace.WriteLine($"{turnContext.Activity.Type}:{turnContext.Activity.Text}", "RingoBot3");
            _logger.LogDebug($"{turnContext.Activity.Type}:{turnContext.Activity.Text}");

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    if (string.IsNullOrEmpty(turnContext.Activity.Text))
                    {
                        Logger.Debug("turnContext.Activity.Text is null or empty. Doing nothing.", nameof(RingoBot3));
                        break;
                    }

                    string text = turnContext.Activity.Text.Trim().ToLower();
                    string command = text.Split(' ')[0];
                    string query = text.Remove(0, command.Length);

                    // login
                    if (command == "login")
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
                            Logger.Debug($"Token = {token2.Token.Substring(0, 5)}...", nameof(RingoBot3));
                            await turnContext.SendActivityAsync($"Already logged in with Token {token2.Token.Substring(0, 5)} 👌. Type `logout` if you would like to logout.");
                        }

                        break;
                    }

                    if (command == "logout")
                    {
                        var adapter = (BotFrameworkAdapter)turnContext.Adapter;
                        await adapter.SignOutUserAsync(turnContext, ConnectionName, cancellationToken: cancellationToken);
                        await turnContext.SendActivityAsync("You have been logged out.");
                        break;
                    }

                    if (command == "play")
                    {
                        var adapter = (BotFrameworkAdapter)turnContext.Adapter;
                        var token2 = await adapter.GetUserTokenAsync(turnContext, ConnectionName, null, cancellationToken);

                        if (token2 == null)
                        {
                            await turnContext.SendActivityAsync("Can't play because I can't find a token. Try `login`.");
                        }
                        else
                        {
                            await _ringoService.PlayPlaylist(
                                turnContext,
                                query,
                                token2.Token,
                                cancellationToken);
                        }

                        break;
                    }

                    // magic number
                    if (_magicCodeRegex.IsMatch(text))
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

                    // any other text
                    if (text.Length > 0)
                    {
                        await turnContext.SendActivityAsync($"You wrote \"{turnContext.Activity.Text}\" but I did not understand 🤔");
                        break;
                    }

                    // do nothing
                    Trace.WriteLine("Doing nothing", "RingoBot3");

                    break;

                case ActivityTypes.ConversationUpdate:
                    if (turnContext.Activity.MembersAdded != null)
                    {
                        await SendWelcomeMessageAsync(turnContext, cancellationToken);
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
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Hi {member.Name}, I'm Ringo! Type `login` to begin.",
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
                        new CardAction
                        {
                            Title = "Sign in link",
                            Text = "Try this link if Sign-in button does not work. (Opens in your browser)",
                            Value = link,
                            Type = ActionTypes.OpenUrl,
                        },
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
