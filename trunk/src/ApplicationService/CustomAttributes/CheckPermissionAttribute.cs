using System;
using System.Linq;
using System.Security.Authentication;
using System.Web.Mvc;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckPermissionAttribute : ActionFilterAttribute
    {
        private readonly string _parameter;
        private readonly IConfiguration _configuration = new BasicConfiguration();
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<AdviceBase> _adviceRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter">Id of advice that should be checked for permission to be modified by the current user.</param>
        public CheckPermissionAttribute(string parameter)
        {
            _parameter = parameter;
            IContainer container = _configuration.Container;
            _userRepository = container.Resolve<IRepository<User>>();
            _adviceRepository = container.Resolve<IRepository<AdviceBase>>();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionParameters.ContainsKey(_parameter))
            {
                var adviceId = (int)filterContext.ActionParameters[_parameter];
                var advice = _adviceRepository.FindOne(x => x.Id == adviceId);

                var user = filterContext.HttpContext.User;
                var shopgunUser = _userRepository.Find(x => x.UserName == user.Identity.Name).FirstOrDefault();

                if (advice.Mentor.Id != shopgunUser.MentorId && !user.IsInRole("SuperUser"))
                {
                    throw new AuthenticationException("You do not have permission to access this information.");
                }
            }

        }
    }
}
