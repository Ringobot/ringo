﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace RingoBotNet.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> Authorize(ITurnContext turnContext, CancellationToken cancellationToken);

        Task<TokenResponse> GetAccessToken(string channelUserId);

        Task<TokenResponse> ValidateMagicNumber(ITurnContext turnContext, string text, CancellationToken cancellationToken);

        Task ResetAuthorization(ITurnContext turnContext, CancellationToken cancellationToken);
    }
}
