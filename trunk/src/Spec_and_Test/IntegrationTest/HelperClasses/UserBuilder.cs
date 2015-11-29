using System;
using Consumentor.ShopGun.Domain;

namespace IntegrationTest.HelperClasses
{
    public class UserBuilder
    {
        public static User BuildUser()
        {
            return new User
                       {
                           CreationDate = DateTime.Now.AddDays(-30.0),
                           DisplayName = "Apan",
                           Email = "a@p.an",
                           FirstName = "Ap",
                           LastName = "An",
                           UserName = "napA",
                           LastActivity = DateTime.Now,
                           LastLockedOutDate = DateTime.Now.AddDays(-1.0),
                           LastLoginDate = DateTime.Now.AddHours(-1.0),
                           LastPasswordChangedDate = DateTime.Now.AddDays(-30.0),
                           IsLockedOut = false,
                           Password = "passw0rd"

                       };
        }
    }
}
