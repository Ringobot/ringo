using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ringo.Bot.Net
{
    public class RingoBot2
    {
        private readonly DialogSet _dialogs;
        private const string ConnectionName = "spotify_connection_2";


        public RingoBot2(IStatePropertyAccessor<DialogState> dialogState)
        {
            _dialogs = new DialogSet(dialogState);

            //_dialogs.Add(new WaterfallDialog("authDialog", new WaterfallStep[] { SendOAuthCardAsync, LoginStepAsync }));

        }
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // ...
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                // If the DialogTurnStatus is Empty we should start a new dialog.
                if (results.Status == DialogTurnStatus.Empty)
                {
                    await dialogContext.BeginDialogAsync("details", null, cancellationToken);
                }
            }

            // ...
            // Save the dialog state into the conversation state.
            //await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);

            // Save the user profile updates into the user state.
            //await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        ////private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //private static async Task<DialogTurnResult> SendOAuthCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var message = stepContext.Activity.CreateReply();

        //    if (message.Attachments == null)
        //    {
        //        message.Attachments = new List<Attachment>();
        //    }

        //    message.Attachments.Add(new Attachment
        //    {
        //        ContentType = OAuthCard.ContentType,
        //        Content = new OAuthCard
        //        {
        //            Text = "Please sign in",
        //            ConnectionName = ConnectionName,
        //            Buttons = new[]
        //            {
        //                new CardAction
        //                {
        //                    Title = "Sign In",
        //                    Text = "Sign In",
        //                    Type = ActionTypes.Signin,
        //                },
        //            },
        //        },
        //    });


        //    await turnContext.SendActivityAsync(message, cancellationToken).ConfigureAwait(false);
        //}

        // This can be called when the bot receives an Activity after sending an OAuthCard
        private async Task<TokenResponse> RecognizeTokenAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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
            else if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // make sure it's a 6-digit code
                var matched = Regex.IsMatch(turnContext.Activity.Text, "^[0-9]{6}$");
                if (matched)
                {
                    var token = await adapter.GetUserTokenAsync(
                        turnContext,
                        ConnectionName,
                        turnContext.Activity.Text,
                        cancellationToken);
                    return token;
                }
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
