//using System.Threading.Tasks;
//using SpotifyApi.NetCore;

//namespace RingoBotNet
//{
//    internal class AccountsService : IAccountsService
//    {
//        protected readonly BearerAccessToken _token;
//        public AccountsService(string token)
//        {
//            _token = new BearerAccessToken{AccessToken=token};
//        }

//        public Task<BearerAccessToken> GetAppAccessToken() => Task.FromResult(_token);
//    }

//    internal class UserAccountsService : AccountsService, IUserAccountsService
//    {
//        public UserAccountsService(string token) : base(token)
//        {
//        }

//        public string AuthorizeUrl(string state, string[] scopes)
//        {
//            throw new System.NotImplementedException();
//        }

//        public Task<BearerAccessToken> GetUserAccessToken(string userHash) => Task.FromResult(_token);

//        public Task<BearerAccessRefreshToken> RequestAccessRefreshToken(string userHash, string code)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}