using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.ApplicationService.Server;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Component;
using System.Configuration.Provider;
using System.Web.Security;

namespace Consumentor.ShopGun.Web.Security
{
    public sealed class ShopgunRoleProvider : RoleProvider
    {
        private IRoleProviderApplicationService _roleProviderApplicationService;
        private IMembershipProviderApplicationService _membershipProviderApplicationService;
        private readonly IConfiguration _configuration = new BasicConfiguration();

        //
        // Global connection string, generic exception message, event log info.
        //
        private string _eventSource = "ShopgunRoleProvider";
        private string _eventLog = "Application";
        private string _exceptionMessage = "An exception occurred. Please check the Event Log.";

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool _pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog
        {
            get { return _pWriteExceptionsToEventLog; }
            set { _pWriteExceptionsToEventLog = value; }
        }


        /// <summary>
        /// Provider.ProviderBase Initialize Method.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            IContainer container = _configuration.Container;
            _roleProviderApplicationService = container.Resolve<IRoleProviderApplicationService>();
            _membershipProviderApplicationService = container.Resolve<IMembershipProviderApplicationService>();

            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "ShopgunRoleprovider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Shopgun Role provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);


            if (config["applicationName"] == null || config["applicationName"].Trim() == "")
            {
                _applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            }
            else
            {
                _applicationName = config["applicationName"];
            }


            if (config["writeExceptionsToEventLog"] == null) return;
            if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
            {
                _pWriteExceptionsToEventLog = true;
            }
        }


        #region System.Web.Security.RoleProvider properties.

        private string _applicationName;
        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }
        #endregion


        #region System.Web.Security.RoleProvider methods.

        public override void CreateRole(string roleName)
        {
            _roleProviderApplicationService.CreateRole(roleName);
        }

        public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
        {
            foreach (string rolename in roleNames)
            {
                if (!RoleExists(rolename))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            foreach (string username in userNames)
            {
                if (username.IndexOf(',') > 0)
                {
                    throw new ArgumentException("User names cannot contain commas.");
                }

                foreach (string rolename in roleNames)
                {
                    if (!IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is not in role.");
                    }
                }
            }

            try
            {
                foreach (string userName in userNames)
                {
                    foreach (string roleName in roleNames)
                    {
                        _roleProviderApplicationService.DeleteUserFromRole(userName, roleName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return _roleProviderApplicationService.DeleteRole(roleName, throwOnPopulatedRole);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            char[] charsToTrim = { '%' };
            List<User> userList = _membershipProviderApplicationService.GetAllUsersToList();
            List<string> uirList = new List<string>();

            foreach (User u in userList)
            {
                if (u.UserName.StartsWith(usernameToMatch.TrimEnd(charsToTrim)))
                {
                    if (_roleProviderApplicationService.IsUserInRole(u.UserName, roleName))
                    {
                        uirList.Add(u.UserName);
                    }

                }
            }
            return uirList.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            List<User> userList = _membershipProviderApplicationService.GetAllUsersToList();
            List<string> uirList = new List<string>();

            foreach (User u in userList)
            {              
                    if (_roleProviderApplicationService.IsUserInRole(u.UserName, roleName))
                    {
                        uirList.Add(u.UserName);
                    }             
            }
            return uirList.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return _roleProviderApplicationService.IsUserInRole(username, roleName);
        }

        public override bool RoleExists(string roleName)
        {
            return _roleProviderApplicationService.RoleExists(roleName);
        }

        public override string[] GetAllRoles()
        {
            IEnumerable<Role> allRoles = _roleProviderApplicationService.GetAllRoles();

            List<string> roleNames = new List<string>();

            foreach (Role role in allRoles)
            {
                roleNames.Add(role.RoleName);
            }
            return roleNames.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            return _roleProviderApplicationService.GetRolesForUser(username);
        }

        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!RoleExists(rolename))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            foreach (string username in usernames)
            {
                if (username.IndexOf(',') > 0)
                {
                    throw new ArgumentException("User names cannot contain commas.");
                }

                foreach (string rolename in rolenames)
                {
                    if (IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is already in role.");
                    }
                }
            }

            try
            {
                foreach (string username in usernames)
                {
                    foreach (string rolename in rolenames)
                    {
                        _roleProviderApplicationService.AddUserToRole(username, rolename);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
