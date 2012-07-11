using System.Collections.Generic;
using HTA.ServiceModel;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{
	[RestService("/userauths")]
	public class UserAuths
	{
		public int[] Ids { get; set; }
	}

	public class UserAuthsResponse : IHasResponseStatus
	{
		public UserAuthsResponse()
		{
			this.Users = new List<User>();
			this.UserAuths = new List<UserAuth>();
			this.OAuthProviders = new List<UserOAuthProvider>();
		}
		public CustomUserSession UserSession { get; set; }

		public List<User> Users { get; set; }

		public List<UserAuth> UserAuths { get; set; }

		public List<UserOAuthProvider> OAuthProviders { get; set; }

		public ResponseStatus ResponseStatus { get; set; }
	}

	//Implementation. Can be called via any endpoint or format, see: http://servicestack.net/ServiceStack.Hello/
	public class UserAuthsService : AppServiceBase<UserAuths>
	{
        public IUserRepository UserRepository { get; set; }
        public IAuthsRepository AuthsRepository { get; set; }
		public IDbConnectionFactory DbFactory { get; set; }

		protected override object Run(UserAuths request)
		{
			var response = new UserAuthsResponse {
				UserSession = base.UserSession,
				Users = UserRepository.GetUsers(),
                UserAuths = AuthsRepository.GetUserAuths(),
                OAuthProviders = AuthsRepository.GetUserOAuthProvider(),
			};

			response.UserAuths.ForEach(x => x.PasswordHash = "[Redacted]");
			response.OAuthProviders.ForEach(x => 
				x.AccessToken = x.AccessTokenSecret = x.RequestTokenSecret = "[Redacted]");

			return response;
		}
	}


}