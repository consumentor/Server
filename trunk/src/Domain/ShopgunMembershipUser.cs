using System;
using System.Web.Security;

namespace Consumentor.ShopGun.Domain
{
    public class ShopgunMembershipUser : MembershipUser
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Mentor Mentor { get; set; }

        public ShopgunMembershipUser()
        {

        }
        public ShopgunMembershipUser(string providername,
                                  string username,
                                  object providerUserKey,
                                  string email,
                                  string passwordQuestion,
                                  string comment,
                                  bool isApproved,
                                  bool isLockedOut,
                                  DateTime creationDate,
                                  DateTime lastLoginDate,
                                  DateTime lastActivityDate,
                                  DateTime lastPasswordChangedDate,
                                  DateTime lastLockedOutDate,
                                  int userId,
                                  string displayName,
                                  string password,
                                  string firstName,
                                  string lastName,
                                  Mentor mentor) :
            base(providername,
                                       username,
                                       providerUserKey,
                                       email,
                                       passwordQuestion,
                                       comment,
                                       isApproved,
                                       isLockedOut,
                                       creationDate,
                                       lastLoginDate,
                                       lastActivityDate,
                                       lastPasswordChangedDate,
                                       lastLockedOutDate)
        {

            UserId = userId;
            DisplayName = displayName;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Mentor = mentor;

        }

        //extensions
        public User ToUser()
        {
            //
            User user = new User
                            {
                                Id = UserId,
                                UserName = UserName,
                                Password = Password,
                                Email = Email,
                                FirstName = FirstName,
                                LastName = LastName,
                                CreationDate = CreationDate,
                                DisplayName = DisplayName,
                                IsLockedOut = IsLockedOut,
                                LastActivity = LastActivityDate,
                                LastLockedOutDate = LastLockoutDate,
                                LastLoginDate = LastLoginDate,
                                LastPasswordChangedDate = LastPasswordChangedDate,
                                MentorId = Mentor != null ? Mentor.Id : (int?) null,
                                Mentor = Mentor
                            };

            return user;   
         }
    }
}
