using System;
using System.Web;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.Web.Context
{
    /// <summary>
    /// Access object for both Application and Session context.
    /// </summary>
    [Serializable]
    public class ShopgunWebContext //: ContextBase
    {
        private ShopgunWebContext()
        {
        }
        public bool IsLoggedIn 
        { 
            get 
            { 
                return HttpContext.Current.User.Identity.IsAuthenticated;
            } 
        }

        /// <summary>
        /// Get or set the current user.
        /// </summary>
        public ShopgunMembershipUser User
        {
            get
            {
                return SessionContext.Current.User;
            }
            set
            {
                SessionContext.Current.User = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static ShopgunWebContext Current
        {
            get
            {
                return new ShopgunWebContext();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void CreateNewApplicationContext()
        {
            ApplicationContext appContext = new ApplicationContext();
            HttpContext.Current.Application["ApplicationContext"] = appContext;
        }
        /// <summary>
        /// 
        /// </summary>
        public static void CreateNewSessionContext()
        {
            SessionContext sessionContext = new SessionContext();
            HttpContext.Current.Session["SessionContext"] = sessionContext;
        }
    }
}