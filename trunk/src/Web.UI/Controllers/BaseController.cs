using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public abstract class BaseController : Controller
    {
        private IMembershipProviderApplicationService _membershipProviderApplicationService;
        private IMentorApplicationService _mentorApplicationService;

        public IMembershipProviderApplicationService MembershipProviderApplicationService
        {
            get { return _membershipProviderApplicationService; }
            set { _membershipProviderApplicationService = value; }
        }

        public IMentorApplicationService MentorApplicationService
        {
            get { return _mentorApplicationService; }
            set { _mentorApplicationService = value; }
        }

        public ShopgunMembershipUser CurrentUser
        {
            get
            {
                if (User != null && !string.IsNullOrEmpty(User.Identity.Name))
                {
                    return _membershipProviderApplicationService.GetUser(User.Identity.Name, false,
                                                                         "ShopgunMembershipProvider") as ShopgunMembershipUser;
                }
                return null;
            }
        }

        public Mentor CurrentMentor
        {
            get
            {
                if(User.IsInRole("SuperUser"))
                {
                    var mentorId = Session["MentorId"] ?? CurrentUser.Mentor.Id;
                    return _mentorApplicationService.GetMentorById(int.Parse(mentorId.ToString()));
                }    
                else
                {
                    return _mentorApplicationService.GetMentorById(CurrentUser.Mentor.Id);
                }
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var action = filterContext.Result as ViewResult;
            if (action != null)
            {
                if (User.IsInRole("SuperUser"))
                {
                    action.MasterName = "Admin";
                    action.ViewData["isSuperUser"] = true;
                }
                else
                {
                    action.MasterName = "Site";
                    action.ViewData["isSuperUser"] = false;
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
