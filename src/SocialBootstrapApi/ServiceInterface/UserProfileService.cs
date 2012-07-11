using HTA.ServiceModel;
using ServiceStack.Common;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;
using SocialBootstrapApi.Models;

namespace SocialBootstrapApi.ServiceInterface
{

    public class UserProfileResponse : IHasResponseStatus
    {
        public UserProfileResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        public UserProfile Result { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }

    public class UserProfileService : RestServiceBase<UserProfile>
    {
        public IUserRepository UserRepository { get; set; }

        public override object OnGet(UserProfile request)
        {
            var session = this.GetSession();

            var userProfile = session.TranslateTo<UserProfile>();
            userProfile.Id = int.Parse(session.UserAuthId);

            //var user = DbFactory.Exec(dbCmd => dbCmd.QueryById<User>(userProfile.Id));
            var user = UserRepository.SelectByProfile(userProfile);
            userProfile.PopulateWith(user);

            return new UserProfileResponse
            {
                Result = userProfile
            };
        }
    }

}