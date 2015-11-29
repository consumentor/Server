using System.Web;

namespace MovieApp.Infrastructure
{
    public static class UserViewSettings
    {
        public static bool EditAllowed
        {
            get
            {
                return HttpContext.Current.User.IsInRole(MovieAppRoles.AdminRoleName);
            }
        }
        public static bool CreateAllowed
        {
            get
            {
                return HttpContext.Current.User.IsInRole(MovieAppRoles.AdminRoleName);
            }
        }

        public static bool DeleteAllowed
        {
            get
            {
                return HttpContext.Current.User.IsInRole(MovieAppRoles.AdminRoleName);
            }
        }
    }
}
