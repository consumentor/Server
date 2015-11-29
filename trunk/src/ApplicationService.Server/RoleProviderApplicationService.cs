using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;
using Consumentor.ShopGun.Domain;
using Castle.Core.Logging;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Component;
using System.Web.Security;
using System.Configuration.Provider;
using System;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class RoleProviderApplicationService : IRoleProviderApplicationService
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UsersInRole> _usersInRoleRepository;

        public ILogger Log { get; set; }

        private readonly IMembershipProviderApplicationService _membershipProviderApplicationService;
        private readonly IConfiguration _configuration = new BasicConfiguration();

        public RoleProviderApplicationService(IRepository<Role> roleRepository, IRepository<UsersInRole> usersInRolesrepository)
        {
            _roleRepository = roleRepository;
            _usersInRoleRepository = usersInRolesrepository;

            IContainer container = _configuration.Container;
            _membershipProviderApplicationService = container.Resolve<IMembershipProviderApplicationService>();
        }

        public void CreateRole(string roleName)
        {
            if (RoleExists(roleName))
            {
                throw new ProviderException("Role already exists.");
            }
            Role newRole = new Role
                               {
                                   RoleName = roleName,
                                   RoleDescription = "N/A"
                               };

            _roleRepository.Add(newRole);
            _roleRepository.Persist();
        }

        public IEnumerable<Role> GetAllRoles()
        {
            return _roleRepository.Find(r => r.RoleName != null);
        }

        public bool RoleExists(string roleName)
        {
            var result = _roleRepository.Find(r => r.RoleName == roleName);
            return result.Count() > 0;
        }

        public bool IsUserInRole(string username, string roleName)
        {
            ShopgunMembershipUser user = _membershipProviderApplicationService.GetUser(username, false, "ShopgunMembershipProvider") as ShopgunMembershipUser;

            Role role = _roleRepository.FindOne(r => r.RoleName == roleName);

            var userInRoleResult = _usersInRoleRepository.Find(u => (u.Role == role) && (u.User.Id == user.UserId));

            return userInRoleResult.Count() > 0;
        }

        public bool AddUserToRole(string userName, string roleName)
        {
            //NOTE:Why do we get a ShopgunMembershipUser instead of a user??
            var membershipUser = _membershipProviderApplicationService.GetUser(userName, false, "ShopgunMembershipProvider") as ShopgunMembershipUser;
            
            var role = _roleRepository.FindOne(r => r.RoleName == roleName);
            role = _usersInRoleRepository.FindDomainObject(role);

            if (membershipUser != null)
            {
                var user = _usersInRoleRepository.FindDomainObject(membershipUser.ToUser());
                var usersInRole = new UsersInRole
                                         {
                                             User =  user,
                                             Role = role
                                         };
             
                _usersInRoleRepository.Add(usersInRole);
                _usersInRoleRepository.Persist();
                return true;
            }

            return false;
        }

        public bool DeleteUserFromRole(string userName, string roleName)
        {
            ShopgunMembershipUser user = _membershipProviderApplicationService.GetUser(userName, false, "ShopgunMembershipProvider") as ShopgunMembershipUser;
            Role role = _roleRepository.FindOne(r => r.RoleName == roleName);
            if (user != null)
            {
                UsersInRole usersInRole = _usersInRoleRepository.FindOne(u => u.Role == role && u.User.Id == user.UserId);
                _usersInRoleRepository.Delete(usersInRole);
                _usersInRoleRepository.Persist();
                return true;
            }
            return false;

        }

        public string[] GetRolesForUser(string username)
        {
            IEnumerable<Role> roles = GetAllRoles();

            List<string> userRoles = new List<string>();

            foreach (Role r in roles)
            {
                if (IsUserInRole(username, r.RoleName))
                {
                    userRoles.Add(r.RoleName);
                }
            }

            return userRoles.ToArray();
        }


        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            bool result = false;

            if (throwOnPopulatedRole == false)
            {
                List<User> allUsers = _membershipProviderApplicationService.GetAllUsersToList();

                foreach (User u in allUsers)
                {
                    if (IsUserInRole(u.UserName, roleName))
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                try
                {
                    Role role = _roleRepository.FindOne(r => r.RoleName == roleName);
                    _roleRepository.Delete(role);
                    _roleRepository.Persist();
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }


    }
}
