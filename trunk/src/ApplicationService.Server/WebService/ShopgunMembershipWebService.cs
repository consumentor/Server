using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Web;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.ApplicationService.WebService;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Gateway.Server;

namespace Consumentor.ShopGun.ApplicationService.Server.WebService
{
    [ServiceBehavior(Namespace = Base.DataContractNamespace)]
    public class ShopgunMembershipWebService : WebServiceBase, IShopgunMembershipContract
    {
        private readonly IMembershipProviderApplicationService _membershipProviderApplicationService;
        private const string ProviderName = "ClientAuthenticationMembershipProvider";

        public ShopgunMembershipWebService()
        {
            _membershipProviderApplicationService = Container.Resolve<IMembershipProviderApplicationService>();
        }

        public ShopgunMembershipWebService(IMembershipProviderApplicationService membershipProviderApplicationService)
        {
            _membershipProviderApplicationService = membershipProviderApplicationService;
        }

        public ShopgunMembershipWebserviceGateway ValidateMobileUser(string username, string password)
        {
            ShopgunMembershipWebserviceGateway response = new ShopgunMembershipWebserviceGateway
                                        {
                                            value = _membershipProviderApplicationService.ValidateMobileUser(username, password)
                                        };

            //TODO: Shall support language, translate from resource string.
            response.message = response.value == false ? "Username or password is wrong!" : "Login successful!";

            if (response.value)
            {
                response.token = _membershipProviderApplicationService.GenerateToken();
                _membershipProviderApplicationService.AddTokenToCache(response.token, username);
            }
            

            return response;
        }

        public ShopgunMembershipWebserviceGateway CreateUser(string username, string password, string email)
        {
            ShopgunMembershipWebserviceGateway response = new ShopgunMembershipWebserviceGateway();
           
            User newUser = new User
                               {
                                   UserName = username,
                                   Password = password,
                                   Email = email,
                                   CreationDate = DateTime.Now,
                                   LastActivity = DateTime.Now,
                                   LastLockedOutDate = DateTime.Now,
                                   LastLoginDate = DateTime.Now,
                                   LastPasswordChangedDate = DateTime.Now
                               };

            //TODO: Shall support language, translate from resource string.

            //Refactoring here please, extract to a method: ValidateNewUserInfo(string username, string password, string email) : bool
            if ((newUser.UserName == null) || (newUser.Password == null))
            {
                response.value = false;
                response.message = "Enter username and password!";
                return response;
            }
            if (_membershipProviderApplicationService.GetUser(newUser.UserName, false, ProviderName) != null)
            {
                response.value = false;
                response.message = "Username already exists!";
                return response;

            }
            if (_membershipProviderApplicationService.GetUserByMail(newUser.Email, ProviderName) != null)
            {
                response.value = false;
                response.message = "Email already exists!";
                return response;
            }
            //End of refactoring

            try
            {
                _membershipProviderApplicationService.CreateUser(newUser);
                response = ValidateMobileUser(newUser.UserName, password);
                if (response.value)
                {
                    response.message = "User created!";   
                }
                return response;
            }
            catch
            {
                response.value = false;
                response.message = "User not created, system error!";
                return response;
            }
        }

        public ShopgunMembershipWebserviceGateway CreateNewUser(string username, string password, string email)
        {
            return CreateUser(username, password, email);
        }

        public ShopgunMembershipWebserviceGateway ValidateGoogleUser()
        {
            var parameters = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;
            if (parameters.AllKeys.Contains("authToken"))
            {
                var authToken = parameters["authToken"];
                var url = "https://www.googleapis.com/userinfo/v2/me";

                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Headers.Add("Authorization", "Bearer " + authToken);

                var response = (HttpWebResponse) request.GetResponse();
                var resStream = response.GetResponseStream();
                
                var googleUser = Activator.CreateInstance<GoogleUser>();
                var jsonSerializer = new DataContractJsonSerializer(googleUser.GetType());
                
                var result = new ShopgunMembershipWebserviceGateway();
                if (resStream != null)
                {
                    googleUser = (GoogleUser) jsonSerializer.ReadObject(resStream);
                    resStream.Close();

                    if (googleUser == null)
                    {
                        result.value = false;
                        result.message = "Could not retrieve user info from Google!";
                    }

                    var user = _membershipProviderApplicationService.GetUserByMail(googleUser.email, ProviderName);

                    if (user != null)
                    {
                        result.value = true;
                        result.message = "Login successful!";
                    }
                    else
                    {
                        user = _membershipProviderApplicationService.CreateUser(new User
                                                                             {
                                                                                 UserName = googleUser.email,
                                                                                 FirstName = googleUser.given_name,
                                                                                 LastName = googleUser.family_name,
                                                                                 Email = googleUser.email,
                                                                                 Password = ""
                                                                             });
                        result.value = true;
                        result.message = "User created!";
                    }
                    if (result.value)
                    {
                        result.token = _membershipProviderApplicationService.GenerateToken();
                        _membershipProviderApplicationService.AddTokenToCache(result.token, user.UserName);
                    }
                    return result;
                }
            }
            System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.Unauthorized;
            return null;
        }
    }

    [DataContract]
    class GoogleUser
    {
        [DataMember]
        public String name { get; set; }

        [DataMember]
        public String email { get; set; }

        [DataMember]
        public String id { get; set; }
        
        [DataMember]
        public String given_name { get; set; }

        [DataMember]
        public String family_name { get; set; }
    }
}
