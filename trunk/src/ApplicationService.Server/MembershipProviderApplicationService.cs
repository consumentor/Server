using System;
using System.Security;
using System.Web.Security;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Web.Caching;
using System.Web;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class MembershipProviderApplicationService : IMembershipProviderApplicationService
    {
        private readonly IRepository<User> _userRepository;
        private const string EncryptionKey = "5f24212ffb14a775449cce8ac5cc3e158ded53d91cd33248";       

        public MembershipProviderApplicationService(IRepository<User> repository)
        {
            _userRepository = repository;
        }

        public ILogger Log { get; set; }

        public int MinPasswordLength
        {
            //TODO config file?
            get { return 5; }
        }

        public MembershipUser GetUser(string username, bool userIsOnline, string providerName)
        {
            try
            {
                var users = _userRepository.Find(u => u.UserName == username);

                if (!users.Any())
                {
                    users = _userRepository.Find(u => u.Email == username);
                }
                var user = users.FirstOrDefault();
                if (user != null)
                {
                    ShopgunMembershipUser shopgunUser = new ShopgunMembershipUser(providerName, user.UserName,
                                                                                  user.Id.ToString(),
                                                                                  user.Email, "", "", true,
                                                                                  user.IsLockedOut,
                                                                                  user.CreationDate, user.LastLoginDate,
                                                                                  user.LastActivity,
                                                                                  user.LastPasswordChangedDate,
                                                                                  user.LastLockedOutDate,
                                                                                  user.Id, user.DisplayName,
                                                                                  user.Password,
                                                                                  user.FirstName, user.LastName, user.Mentor);

                    return shopgunUser;
                }
                return null;
                //Fallback for umbraco back office users
                //return GetUserFromFallbackMembershipProvider(username, userIsOnline, "UsersMembershipProvider");
            }
            catch(Exception e)
            {
                throw new Exception(e.Message,e);
            }
        }

        private MembershipUser GetUserFromFallbackMembershipProvider(string username, bool userIsOnline, string providerName)
        {
            MembershipProvider usersMembershipProvider = Membership.Providers[providerName];
            if(usersMembershipProvider != null)
            {
                MembershipUser usersMembershipUser = usersMembershipProvider.GetUser(username, userIsOnline);
                return usersMembershipUser;
            }
            throw  new SecurityException("UsersMembershipProvider is not initialized or configurated in a correct manner!");
        }

        public MembershipUser GetUserById(int id, bool userIsOnline, string providerName)
        {
            try
            {
                var users = _userRepository.Find(u => u.Id == id);
                User user = users.FirstOrDefault();
                ShopgunMembershipUser shopgunUser = new ShopgunMembershipUser(providerName, user.UserName, new Guid(),
                                                                              user.Email, "", "", true, user.IsLockedOut,
                                                                              user.CreationDate, user.LastLoginDate,
                                                                              user.LastActivity,
                                                                              user.LastPasswordChangedDate,
                                                                              user.LastLockedOutDate,
                                                                              user.Id, user.DisplayName, user.Password,
                                                                              user.FirstName, user.LastName, user.Mentor);

                return shopgunUser;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        
        public MembershipUser GetUserByMail(string email, string providerName)
        {
            try
            {
                var users = _userRepository.Find(u => u.Email == email);
                User user = users.FirstOrDefault();

                if (user != null)
                {
                    ShopgunMembershipUser shopgunUser = new ShopgunMembershipUser(providerName, user.UserName,
                                                                                  new Guid(),
                                                                                  user.Email, "", "", true,
                                                                                  user.IsLockedOut,
                                                                                  user.CreationDate, user.LastLoginDate,
                                                                                  user.LastActivity,
                                                                                  user.LastPasswordChangedDate,
                                                                                  user.LastLockedOutDate,
                                                                                  user.Id, user.DisplayName,
                                                                                  user.Password,
                                                                                  user.FirstName, user.LastName, user.Mentor);

                    return shopgunUser;
                }
                //TODO: Check if email exist in the Fallback Membership Provider.
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public bool ValidateUser(string username, string password)
        {
            password = EncodePassword(password);
            User user;
            try
            {
                user = _userRepository.Find(u => u.UserName == username).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

            if (user != null && (password == user.Password) && (username == user.UserName))
            {
                try
                {
                    user.LastLoginDate = DateTime.Now;
                    _userRepository.Persist();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
                return true;
            }
            return false;
        }

        public void UpdateLastActivity(string userName)
        {
            try
            {
                User user = _userRepository.FindOne(u => u.UserName == userName);
                user.LastActivity = DateTime.Now;
                _userRepository.Persist();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            
        }
             

        public bool ValidateMobileUser(string username, string password)
        {
            User user;
            password = EncodePassword(password);
            try
            {
                var users = _userRepository.Find(u => u.UserName == username);
                if (users.Any())
                {
                    user = users.Single();
                }
                else
                {
                    user = _userRepository.Find(u => u.Email == username).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

            if (user != null && (password == user.Password) && (username == user.UserName || username == user.Email ))
            {
                try
                {
                    user.LastLoginDate = DateTime.Now;
                    _userRepository.Persist();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
                return true;
            }
            return false;
        }

        public MembershipUser CreateUser(User newUser)
        {
            newUser.Password = EncodePassword(newUser.Password);
            newUser.CreationDate = DateTime.Now;
            newUser.LastActivity = DateTime.Now;
            newUser.LastPasswordChangedDate = DateTime.Now;
            newUser.LastLoginDate = DateTime.Now;
            newUser.LastLockedOutDate = DateTime.MaxValue;

            try
            {
                _userRepository.Add(newUser);
                _userRepository.Persist();
                return newUser.ToMembershipUser();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public MembershipUser CreateUser(string userName, string password, string email)
        {
            var newUser = new User
                              {
                                  UserName = userName,
                                  Email = email,
                                  Password = EncodePassword(password),
                                  CreationDate = DateTime.Now,
                                  LastActivity = DateTime.Now,
                                  LastPasswordChangedDate = DateTime.Now,
                                  LastLoginDate = DateTime.Now,
                                  LastLockedOutDate = DateTime.MaxValue
                              };
            try
            {
                _userRepository.Add(newUser);
                _userRepository.Persist();
                return newUser.ToMembershipUser();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            User user;
            try
            {
                var users = _userRepository.Find(u => u.UserName == username);
                user = users.FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

            if (username == user.UserName)
            {
                try
                {
                    _userRepository.Delete(user);
                    _userRepository.Persist();
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
            return false;
        }

        public bool UpdateUser(User updatedUser, bool updateUserActivity) 
        {
            User user = _userRepository.FindOne(u => u.Id == updatedUser.Id);

            updatedUser.Password = updatedUser.Password != null ? EncodePassword(updatedUser.Password) : user.Password;

            user.CopyStringProperties(updatedUser);
            SetMentor(user, updatedUser.MentorId);

            if (updateUserActivity)
            {
                user.CopyDateTimeProperties(updatedUser);
            }
            
            try
            {                
                _userRepository.Persist();                                        
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        private void SetMentor(User user, int? mentorId)
        {
            if (mentorId != null)
            {
                var mentor = _userRepository.FindDomainObject(new Mentor {Id = mentorId.Value});
                user.Mentor = mentor;
            }
            else
            {
                user.Mentor = null;
            }
        }

        public User[] GetAllUsersToArray()
        {
            var users = _userRepository.Find(u => u.UserName != null);
            return users.ToArray();
        }
        
        public List<User> GetAllUsersToList()
        {
            var users = _userRepository.Find(u => u.UserName != null);
            return users.ToList();
        }

        public string GenerateToken()
        {
            char[] tokenCharArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~!@$&*".ToCharArray();
            const int tokenLenght = 30;
            StringBuilder tokenBuilder = new StringBuilder();
            Random rnd = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < tokenLenght; i++)
            {
                tokenBuilder.Append(tokenCharArray[rnd.Next(0, tokenCharArray.Length)].ToString());
            }

            return tokenBuilder.ToString();
        }

        public bool AddTokenToCache(string token, string userName)
        {
            MembershipUser u;
            
            try
            {
                u = GetUser(userName, false, "ClientAuthenticationMembershipProvider");
                HttpRuntime.Cache.Insert(token, u.ProviderUserKey.ToString(), null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            
            return (u.UserName == userName) || (HttpRuntime.Cache.Get(token).ToString() == u.ProviderUserKey.ToString());
        }

        public virtual bool ValidateToken(string token)
        {
            object result = HttpRuntime.Cache.Get(token);
            return result != null;
        }

        public User GetUserForToken(string token)
        {
            User user = null;
            if (ValidateToken(token))
            {
                var userId = int.Parse((string) HttpRuntime.Cache.Get(token));
                user = _userRepository.Find(x => x.Id == userId).FirstOrDefault();
            }
            return user;
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = _userRepository.FindOne(x => x.UserName == username);
            if (user.Password == EncodePassword(oldPassword))
            {
                user.Password = EncodePassword(newPassword);
                _userRepository.Persist();
                return true;
            }
            return false;
        }


        //TODO: Shall be move to the infrastructe project.
        // Help methods
        private string EncodePassword(string password)
        {
            HMACSHA1 hash = new HMACSHA1 { Key = HexToByte(EncryptionKey) };
            string encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));

            return encodedPassword;
        }
        
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }   

    }
}
