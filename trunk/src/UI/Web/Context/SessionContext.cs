using System.Globalization;
using System.Web;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.Web.Context
{
    /// <summary>
    /// 
    /// </summary>
    internal class SessionContext
    {
        public SessionContext()
        {
            CultureName = "sv-SE"; // Swedish - Sweden
            CultureInformation = new CultureInfo(CultureName, false);
            LanguageId = "1";
            User = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static SessionContext Current
        {
            get
            {
                SessionContext sessionContext = (SessionContext)HttpContext.Current.Session["SessionContext"];
                if (sessionContext == null)
                {
                    sessionContext = new SessionContext();
                    HttpContext.Current.Session["SessionContext"] = sessionContext;                    
                }
                return (SessionContext)HttpContext.Current.Session["SessionContext"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string CultureName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CultureInfo CultureInformation { get; private set; }

        /// <summary>
        /// Get or sets the current auhenticated user.
        /// </summary>
        public ShopgunMembershipUser User { get; set; }
    }
}
