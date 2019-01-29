using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ringo.Bot.Net.Models;
using Ringo.Bot.Net.Services;
using Ringo.Bot.Net.State;
using SpotifyApi.NetCore;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Ringo.Bot.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly IUserAccountsService _userAccounts;
        private readonly UserAuthService _authService;
        private readonly AuthStateService _stateService;
        private readonly RingoBotAccessors _stateAccessors;

        public SpotifyController (
            IUserAccountsService userAccounts,
            UserAuthService authService,
            AuthStateService stateService,
            RingoBotAccessors accessors)
        {
            _userAccounts = userAccounts;
            _authService = authService;
            _stateService = stateService;
            _stateAccessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
        }

        private async Task<string> GetAccessToken() =>
            (await _userAccounts.GetUserAccessToken(GetUserId())).AccessToken;

        //[HttpPost("[action]")]
        //[Route("api/spotify/authorize")]
        //public SpotifyAuthorization Authorize()
        //{
        //    string userId = GetUserId();

        //    var userAuth = _authService.GetUserAuth(userId);
        //    if (userAuth != null && userAuth.Authorized) return MapToSpotifyAuthorization(userAuth);
        //    if (userAuth == null) userAuth = _authService.CreateUserAuth(userId);

        //    // create a state value and persist it until the callback
        //    string state = _stateService.NewState(userId);

        //    // generate an Authorization URL for the read and modify playback scopes
        //    string url = _userAccounts.AuthorizeUrl(state, new[] { "user-read-playback-state", "user-modify-playback-state" });

        //    return new SpotifyAuthorization
        //    {
        //        UserId = userId,
        //        Authorized = false,
        //        AuthorizationUrl = url
        //    };
        //}

        /// Authorization callback from Spotify
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

            // Use the code to request a token
            BearerAccessRefreshToken token = await _userAccounts.RequestAccessRefreshToken(code);

            var conversationData = await _stateAccessors.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());

            //TODO: var userAuth = _authService.SetUserAuthRefreshToken(userId, tokens);

            //TODO: check state is valid
            //_stateService.ValidateState(state, userId);
            string value;
            if (!_states.TryGetValue(state, out value)) throw new InvalidOperationException("Invalid State value");
            if (value != userId) throw new InvalidOperationException("Invalid State value");


            // return an HTML result that posts a message back to the opening window and then closes itself.
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = $"<html><body><script>window.opener.postMessage(true, \"*\");window.close()</script>Spotify Authorization successful. You can close this window now</body></html>"
            };
        }

        /// Get's the userId cookie and sets one if it does not exist
        private string GetUserId()
        {
            const string UserIdCookieName = "ringobotUserId";
            string id = Request.Cookies[UserIdCookieName];
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString("N");
                Response.Cookies.Append(UserIdCookieName, id,
                    new CookieOptions { Expires = DateTime.Now.AddYears(1), SameSite = SameSiteMode.None });
            }

            return id;
        }

        private SpotifyAuthorization MapToSpotifyAuthorization(UserAuth userAuth)
        {
            return new SpotifyAuthorization
            {
                Authorized = userAuth.Authorized,
                UserId = userAuth.UserId
            };
        }
    }

    public class SpotifyAuthorization
    {
        public string UserId { get; set; }
        public bool Authorized { get; set; }
        public string AuthorizationUrl { get; set; }
    }

}