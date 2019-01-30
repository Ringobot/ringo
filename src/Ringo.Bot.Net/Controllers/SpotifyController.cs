using Microsoft.AspNetCore.Mvc;
using RingoBotNet.Data;
using RingoBotNet.Services;
using SpotifyApi.NetCore;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RingoBotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly IUserAccountsService _userAccounts;
        //private readonly UserAuthService _authService;
        //private readonly AuthStateService _stateService;
        private readonly IChannelUserData _userData;
        private readonly IUserStateData _userStateData;

        public SpotifyController(
            IUserAccountsService userAccounts,
            //UserAuthService authService,
            //AuthStateService stateService,
            IChannelUserData channelUserData,
            IUserStateData userStateData)
        {
            _userAccounts = userAccounts;
            //_authService = authService;
            //_stateService = stateService;
            _userData = channelUserData;
            _userStateData = userStateData;
        }

        [HttpGet("[action]")]
        [Route("api/spotify/authorize")]
        public async Task<ContentResult> Authorize(
            [FromQuery(Name = "state")] string state,
            [FromQuery(Name = "code")] string code = null,
            [FromQuery(Name = "error")] string error = null)
        {
            //string userId = GetUserId();

            // if Spotify returned an error, throw it
            if (error != null) throw new SpotifyApiErrorException(error);

            // validate state
            if (!RingoService.RingoBotStateRegex.IsMatch(state)) throw new ArgumentException("Invalid State Argument", nameof(state));

            // get the userId from state
            string channelUserId = await _userStateData.GetChannelUserIdFromStateToken(state);

            // Use the code to request a token
            BearerAccessRefreshToken token = await _userAccounts.RequestAccessRefreshToken(code);

            await _userData.SaveUserAccessToken(channelUserId, token);

            // return an HTML result with the state token to authorise the bot
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = $"<html><body><p>Copy this code into the chat window:<br/> <text>{state}</text></p></body></html>"
            };
        }
    }
}