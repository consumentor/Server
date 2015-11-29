using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IRoleProviderApplicationService
    {
        void CreateRole(string roleName);
        IEnumerable<Role> GetAllRoles();
        bool RoleExists(string roleName);
        bool IsUserInRole(string username, string roleName);
        bool AddUserToRole(string username, string roleName);
        string[] GetRolesForUser(string username);
        bool DeleteRole(string roleName, bool throwOnPopulatedRole);
        bool DeleteUserFromRole(string userName, string roleName);
    }
}
