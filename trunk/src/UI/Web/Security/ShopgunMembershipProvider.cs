using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Security.Cryptography;
using System.Web.Security;
using System.Text;
using System.Diagnostics;
using Consumentor.ShopGun.ApplicationService.Server;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;
using Consumentor.ShopGun.Domain;


namespace Consumentor.ShopGun.Web.Security
{
    public sealed class ShopgunMembershipProvider : MembershipProvider
    {
        private int _newPasswordLength = 8;

        private const string Ivkey = "c894fabf835ca27e470e7ad479ec5c41";
        private const string EncryptionKey = "5f24212ffb14a775449cce8ac5cc3e158ded53d91cd33248";
        private byte[] _encryptionKeyData;
        private byte[] _ivKeyData;
        private bool _writeExceptionsToEventLog;
        private const string ProviderName = "ShopgunMembershipProvider";

        private IMembershipProviderApplicationService _membershipProviderApplicationService;
        private readonly IConfiguration _configuration = new BasicConfiguration();
       

        /// <summary>
        /// System.Configuration.Provider.ProviderBase.Initialize Method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {            
            _encryptionKeyData = HexToByte(EncryptionKey);
            _ivKeyData = HexToByte(Ivkey);

            IContainer container = _configuration.Container;
            _membershipProviderApplicationService = container.Resolve<IMembershipProviderApplicationService>();
            
            //
            // Initialize values from web.config.
            //


            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "ShopgunMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Shopgun Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            m_ApplicationName = GetConfigValue(config["applicationName"],
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            m_MaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            m_PasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            m_MinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            m_MinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            m_PasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            m_EnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            m_EnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            m_RequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            m_RequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            _writeExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            string tempFormat = config["passwordFormat"];
            if (tempFormat == null)
            {
                tempFormat = "Hashed";
            }

            switch (tempFormat)
            {
                case "Hashed":
                    m_PasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    m_PasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    m_PasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }
        }
        /// <summary>
        /// If false, exceptions are thrown to the caller. If true, exceptions are written to the event log.
        /// </summary>
        public bool WriteExceptionsToEventLog
        {
            get { return _writeExceptionsToEventLog; }
            set { _writeExceptionsToEventLog = value; }
        }

        /// <summary>
        /// A helper function to retrieve config values from the configuration file.
        /// </summary>
        /// <param name="configValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private string GetConfigValue(string configValue, string defaultValue)
        {
            return String.IsNullOrEmpty(configValue) ? defaultValue : configValue;
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }        
        /// <summary>
        /// Compares password values based on the MembershipPasswordFormat.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="dbpassword"></param>
        /// <returns></returns>
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }

            return pass1 == pass2;
        }
        /// <summary>
        /// Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HMACSHA1 hash = new HMACSHA1 {Key = HexToByte(EncryptionKey)};
                    encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }
        /// <summary>
        /// Decrypts or leaves the password clear based on the PasswordFormat.
        /// </summary>
        /// <param name="encodedPassword"></param>
        /// <returns></returns>
        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }
        /// <summary>
        /// A helper function that writes exception detail to the event log. 
        /// Exceptions are written to the event log as a security measure to avoid private database 
        /// details from being returned to the browser. If a method does not return a status 
        /// or boolean indicating the action succeeded or failed, a generic exception is also thrown by the caller.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="action"></param>
        private void WriteToEventLog(Exception e, string action)
        {
            Trace.Write(e);
        }

        #region System.Web.Security.MembershipProvider properties.

        private string m_ApplicationName;
        private bool m_EnablePasswordReset;
        private bool m_EnablePasswordRetrieval;
        private bool m_RequiresQuestionAndAnswer;
        private bool m_RequiresUniqueEmail;
        private int m_MaxInvalidPasswordAttempts;
        private int m_PasswordAttemptWindow;
        private MembershipPasswordFormat m_PasswordFormat;
        private int m_MinRequiredNonAlphanumericCharacters;
        private int m_MinRequiredPasswordLength;
        private string m_PasswordStrengthRegularExpression;

        /// <summary>
        /// Gets or sets the application name.
        /// The name of the application using the custom membership provider.
        /// </summary>
        public override string ApplicationName
        {
            get { return m_ApplicationName; }
            set { m_ApplicationName = value; }
        }
        /// <summary>
        /// Gets the EnablePasswordReset flag which indicates whether the membership provider is configured to allow users to reset their passwords. 
        /// True if the membership provider supports password reset; otherwise, false.
        /// </summary>
        public override bool EnablePasswordReset
        {
            get { return m_EnablePasswordReset; }
        }
        /// <summary>
        /// Gets the EnablePasswordRetrieval flag which indicates whether the membership provider is configured to allow
        /// users to retrieve their passwords.
        /// True if the membership provider is configured to support password retrieval; otherwise, false. The default is false. 
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get { return m_EnablePasswordRetrieval; }
        }
        /// <summary>
        /// Gets the RequiresQuestionAndAnswer flag value. A value indicating whether the membership provider is configured to 
        /// require the user to answer a password question.
        /// True if a password answer is required for password reset and retrieval; otherwise, false. The default is true. 
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get { return m_RequiresQuestionAndAnswer; }
        }
        /// <summary>
        /// Gets the RequiresUniqueEmail flag value. A value indicating whether the membership provider is configured to require
        /// a unique e-mail address for each user name.
        /// True if the membership provider requires a unique e-mail address; otherwise, false. The default is true. 
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get { return m_RequiresUniqueEmail; }
        }
        /// <summary>
        /// Gets the MaxInvalidPasswordAttempts, the number of invalid password or password-answer attempts 
        /// allowed before the membership user is locked out.
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get { return m_MaxInvalidPasswordAttempts; }
        }
        /// <summary>
        /// Gets the PasswordAttemptWindow, the number of minutes in which a maximum number of invalid password or password-answer 
        /// attempts are allowed before the membership user is locked out. 
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get { return m_PasswordAttemptWindow; }
        }
        /// <summary>
        /// Gets the PasswordFormat value.
        /// One of the MembershipPasswordFormat values indicating the format for storing passwords in the data store. 
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return m_PasswordFormat; }
        }
        /// <summary>
        /// Gets the MinRequiredNonAlphanumericCharacters.
        /// The minimum number of special characters that must be present in a valid password. 
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return m_MinRequiredNonAlphanumericCharacters; }
        }
        /// <summary>
        /// Gets the MinRequiredPasswordLength.
        /// The minimum length required for a password. 
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get { return m_MinRequiredPasswordLength; }
        }
        /// <summary>
        /// Gets the PasswordStrengthRegularExpression.
        /// A regular expression used to evaluate a password. 
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get { return m_PasswordStrengthRegularExpression; }
        }

        #endregion

        #region System.Web.Security.MembershipProvider methods.
        /// <summary>
        /// Processes a request to update the password for a membership user. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return _membershipProviderApplicationService.ChangePassword(username, oldPassword, newPassword);
        }
        /// <summary>
        /// Processes a request to update the password question and answer for a membership user. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPasswordQuestion"></param>
        /// <param name="newPasswordAnswer"></param>
        /// <returns></returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Adds a new membership user to the data source. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {             
            //TODO: All code shall be moved into the application service.
            User newUser = new User();

            newUser.UserName = username;
            newUser.Password = password;
            newUser.Email = email;
            newUser.CreationDate = DateTime.Now;
            newUser.LastActivity = DateTime.Now;
            newUser.LastLockedOutDate = DateTime.Now;
            newUser.LastLoginDate = DateTime.Now;
            newUser.LastPasswordChangedDate = DateTime.Now;
                

            if (_membershipProviderApplicationService.GetUser(newUser.UserName, false, ProviderName) != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }
            else if (_membershipProviderApplicationService.GetUserByMail(newUser.Email, ProviderName) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
            }
            else
            {
                try
                {
                    _membershipProviderApplicationService.CreateUser(newUser);
                    status = MembershipCreateStatus.Success;
                    return newUser.ToMembershipUser();
                }
                catch(Exception e)
                {
                    status = MembershipCreateStatus.UserRejected;
                    throw new Exception(e.Message,e);
                }
            }
                      

            return null;
        }


        //Added to user.cs as ToMembershipUser()
        //private MembershipUser ConvertToMembershipUser(User u)
        //{
        //    if (u == null)
        //    {
        //        return null;
        //    }

        //    return new MembershipUser(ProviderName, u.UserName, u.Id, u.Email,
        //                              string.Empty, string.Empty, true, false, DateTime.Now, DateTime.Now,
        //                              DateTime.Now, DateTime.Now, DateTime.Now);
        //}

        /// <summary>
        /// Removes a user from the membership data source. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="deleteAllRelatedData"></param>
        /// <returns></returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return _membershipProviderApplicationService.DeleteUser(username, deleteAllRelatedData);
        }
        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match. 
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match. 
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        
        // check 
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            char[] charsToTrim = {'%'};
            string userName;
            int counter = 0;
            int startIndex = pageSize * pageIndex;
            int endIndex = startIndex + pageSize - 1;
            MembershipUserCollection membersList = new MembershipUserCollection();
            //TODO: Shall we call the service GetAllUsersToArray? A better name should be: GetUsers
            User[] userArray = _membershipProviderApplicationService.GetAllUsersToArray();
            totalRecords = userArray.Length;

            foreach (User u in userArray)
            {
                userName = u.UserName;
                userName.Trim();
                if (userName.StartsWith(usernameToMatch.TrimEnd(charsToTrim)))
                {
                    if (counter >= startIndex)
                        membersList.Add(u.ToMembershipUser());
                    if (counter >= endIndex) break;
                    counter++;
                }
            }
            return membersList;
        }
        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data. 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            int counter = 0;
            int startIndex = pageSize * pageIndex;
            int endIndex = startIndex + pageSize - 1;
            MembershipUserCollection membersList = new MembershipUserCollection();
            User[] userArray = _membershipProviderApplicationService.GetAllUsersToArray();
            totalRecords = userArray.Length;

            foreach (User u in userArray)
            {
                if (counter >= startIndex)
                    membersList.Add(u.ToMembershipUser());
                if (counter >= endIndex) break;
                counter++;
            }
            return membersList;
        }
        /// <summary>
        /// Gets the number of users currently accessing the application. 
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Gets the password for the specified user name from the data source. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string GetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected IContainer Container { get; private set; }
        /// <summary>
        /// Gets information from the data source for a user. 
        /// Provides an option to update the last-activity date/time stamp for the user. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {            
            return _membershipProviderApplicationService.GetUser(username, userIsOnline, Name);
        }
        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. 
        /// Provides an option to update the last-activity date/time stamp for the user. 
        /// </summary>
        /// <param name="providerUserKey"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            string strId = providerUserKey.ToString();

            int id = Int32.Parse(strId);

            return _membershipProviderApplicationService.GetUserById(id, userIsOnline, ProviderName);
            
        }
        /// <summary>
        /// Clears a lock so that the membership user can be validated. 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override bool UnlockUser(string userName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Gets the user name associated with the specified e-mail address. 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override string GetUserNameByEmail(string email)
        {
            return _membershipProviderApplicationService.GetUserByMail(email, ProviderName).UserName;
        }
        /// <summary>
        /// Resets a user's password to a new, automatically generated password. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string ResetPassword(string username, string answer)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /// <summary>
        /// Updates information about a user in the data source. 
        /// </summary>
        /// <param name="user"></param>
        public override void UpdateUser(MembershipUser user)
        {
            ShopgunMembershipUser shopgunUser = user as ShopgunMembershipUser;
            _membershipProviderApplicationService.UpdateUser(shopgunUser.ToUser(), true);
        }
        /// <summary>
        /// Verifies that the specified user name and password exist in the data source. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override bool ValidateUser(string username, string password)
        {        
            return _membershipProviderApplicationService.ValidateUser(username, password);
        }
        /// <summary>
        /// Decrypts an encrypted password. 
        /// </summary>
        /// <param name="encodedPassword"></param>
        /// <returns></returns>
        protected override byte[] DecryptPassword(byte[] encodedPassword)
        {
            return base.DecryptPassword(encodedPassword);
        }
        /// <summary>
        /// Encrypts a password.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        protected override byte[] EncryptPassword(byte[] password)
        {
            return base.EncryptPassword(password);
        }
        #endregion
    }
}
