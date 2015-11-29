using System;
using System.Web;

namespace Consumentor.ShopGun.Web.Context
{
    /// <summary>
    /// 
    /// </summary>
    internal class ApplicationContext
    {
        private Random _random = new Random();
        /// <summary>
        /// 
        /// </summary>
        public ApplicationContext()
        {
            ImagesBasePath = "/img";
        }
        /// <summary>
        /// 
        /// </summary>
        public static ApplicationContext Current
        {
            get
            {
                return (ApplicationContext)HttpContext.Current.Application["ApplicationContext"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ImagesBasePath { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Random Random
        {
            get
            {
                return _random;
            }
        }

    }
}