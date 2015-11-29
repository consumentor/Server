using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class UserController : BaseController
    {
        private readonly IMembershipProviderApplicationService _membershipProviderApplicationService;
        private readonly IMentorApplicationService _mentorApplicationService;
        private readonly IRoleProviderApplicationService _roleProviderApplicationService;

        public UserController(IMembershipProviderApplicationService membershipProviderApplicationService, IMentorApplicationService mentorApplicationService, IRoleProviderApplicationService roleProviderApplicationService)
        {
            _membershipProviderApplicationService = membershipProviderApplicationService;
            _roleProviderApplicationService = roleProviderApplicationService;
            _mentorApplicationService = mentorApplicationService;
        }

        //
        // GET: /User/

        public ActionResult Index()
        {
            var users = _membershipProviderApplicationService.GetAllUsersToList();
            return View(users);
        }

        //
        // GET: /User/Create

        public ActionResult CreateUser()
        {
            var mentors = _mentorApplicationService.GetAllMentors();
            ViewData["Mentor"] = new SelectList(mentors, "Id", "MentorName");
            var roles = _roleProviderApplicationService.GetAllRoles();
            ViewData["Role"] = roles;
            return View();
        } 

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult CreateUser(User newUser, FormCollection formCollection)
        {
            try
            {
                _membershipProviderApplicationService.CreateUser(newUser);

                var roles = formCollection["Roles"].Replace("false","").Split(new[]{","}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var role in roles)
                {
                    _roleProviderApplicationService.AddUserToRole(newUser.UserName, role);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("CreateUser");
            }
        }

        //
        // GET: /User/Edit/5
 
        public ActionResult EditUser(int id)
        {
            var membershipUser = _membershipProviderApplicationService.GetUserById(id, false, "ShopgunMembershipProvider") as ShopgunMembershipUser;
            var user = membershipUser != null ? membershipUser.ToUser() : new User();

            var mentors = _mentorApplicationService.GetAllMentors();
            ViewData["Mentor"] = new SelectList(mentors, "Id", "MentorName");
            var usersRoles = _roleProviderApplicationService.GetRolesForUser(user.UserName);
            ViewData["UsersRoles"] = usersRoles;
            var roles = _roleProviderApplicationService.GetAllRoles();
            ViewData["Role"] = roles;
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult EditUser(User updatedUser, FormCollection formCollection)
        {
            try
            {
                _membershipProviderApplicationService.UpdateUser(updatedUser, false);

                var updatedRoles = formCollection["Roles"].Replace("false", "").Split(new []{','}, StringSplitOptions.RemoveEmptyEntries);
                var usersRoles = _roleProviderApplicationService.GetRolesForUser(updatedUser.UserName);

                var rolesToRemove = usersRoles.Except(updatedRoles);
                foreach (var roleToRemove in rolesToRemove)
                {
                    _roleProviderApplicationService.DeleteUserFromRole(updatedUser.UserName, roleToRemove);
                }

                var rolesToAdd = updatedRoles.Except(usersRoles);
                foreach (var roleToAdd in rolesToAdd)
                {
                    _roleProviderApplicationService.AddUserToRole(updatedUser.UserName, roleToAdd);
                }
 
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("EditUser", new {id = updatedUser.Id});
            }
        }

        [Authorize]
        public ActionResult DeleteUser(int id)
        {
            var userToDelete = _membershipProviderApplicationService.GetUserById(id, false, "ShopgunMembershipProvider");
            _membershipProviderApplicationService.DeleteUser(userToDelete.UserName, true);

            return RedirectToAction("Index");
        }
    }
}
