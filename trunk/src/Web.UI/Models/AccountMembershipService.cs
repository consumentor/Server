using System;
using System.Web.Security;

namespace Consumentor.Shopgun.Web.UI.Models
{
    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            ValidationUtil.ValidateRequiredStringValue(userName, "userName");
            ValidationUtil.ValidateRequiredStringValue(password, "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            ValidationUtil.ValidateRequiredStringValue(userName, "userName");
            ValidationUtil.ValidateRequiredStringValue(password, "password");
            ValidationUtil.ValidateRequiredStringValue(email, "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            ValidationUtil.ValidateRequiredStringValue(userName, "userName");
            ValidationUtil.ValidateRequiredStringValue(oldPassword, "oldPassword");
            ValidationUtil.ValidateRequiredStringValue(newPassword, "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }
}