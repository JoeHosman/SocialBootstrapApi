using System.Collections.Generic;
using ServiceStack.ServiceInterface.Auth;

namespace HTA.ServiceModel
{
    public interface IUserRepository
    {
        void Save(User user);
        User SelectByProfile(UserProfile userProfile);
        List<User> GetUsers();
        List<User> GetUsersByID(int[] userIds);
    }

    public interface IAuthsRepository
    {
        List<UserAuth> GetUserAuths();

        List<UserOAuthProvider> GetUserOAuthProvider();
    }
}