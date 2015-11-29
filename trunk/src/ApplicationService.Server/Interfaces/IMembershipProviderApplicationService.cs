using System.Collections.Generic;
using System.Web.Security;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IMembershipProviderApplicationService
    {
        int MinPasswordLength { get; }
        MembershipUser GetUser(string username, bool userIsOnline, string providerName);
        MembershipUser GetUserById(int id, bool userIsOnline, string providerName);
        MembershipUser GetUserByMail(string email, string providerName);
        bool ValidateUser(string username, string password);
        bool ValidateMobileUser(string username, string password);
        MembershipUser CreateUser(User newUser);
        MembershipUser CreateUser(string userName, string password, string email);
        bool DeleteUser(string username, bool deleteAllRelatedData);
        User[] GetAllUsersToArray();
        List<User> GetAllUsersToList();
        bool UpdateUser(User updatedUser, bool updateUserActivity);
        string GenerateToken();
        bool AddTokenToCache(string token, string userId);
        bool ValidateToken(string token);
        User GetUserForToken(string token);
        bool ChangePassword(string username, string oldPassword, string newPassword);
    }
}
