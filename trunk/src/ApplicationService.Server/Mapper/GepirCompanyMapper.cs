using System;
using System.Linq;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Gateway;
using Consumentor.ShopGun.Gateway.se.gs1.gepir;
using Consumentor.ShopGun.Log;

namespace Consumentor.ShopGun.ApplicationService.Server.Mapper
{
    [Interceptor(typeof(LogInterceptor))]
    public class GepirCompanyMapper : IMapper<Company, partyDataLineType>
    {
        public GepirCompanyMapper()
        {
        }

        public ILogger Log { get; set; }

        public partyDataLineType Map(Company source)
        {
            throw new NotImplementedException();
        }

        public Company Map(partyDataLineType source)
        {
            var address = source.streetAddress == null
                              ? ""
                              : source.streetAddress.Aggregate((current, next) => current + (" " + next));
            var companyName = source.partyName;
            var postCode = source.postalCode;
            var city = source.city;
            var phone = "";
            var email = "";
            var website = "";
            if (source.contact != null)
            {
                foreach (var partyContactType in source.contact)
                {
                    if (partyContactType.communicationChannel != null)
                    {
                        foreach (var communicationChannelType in partyContactType.communicationChannel)
                        {
                            switch (communicationChannelType.communicationChannelCode)
                            {
                                case CommunicationChannelCodeType.TELEPHONE:
                                    phone = communicationChannelType.Value;
                                    break;
                                case CommunicationChannelCodeType.EMAIL:
                                    email = communicationChannelType.Value;
                                    break;
                                case CommunicationChannelCodeType.WEBSITE:
                                    website = communicationChannelType.Value;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            var company = new Company
                              {
                                  Address = address
                                  , City = city
                                  , ContactEmailAddress = email
                                  , CompanyName = companyName
                                  , PhoneNumber = phone
                                  , PostCode = postCode
                                  , URLToHomePage = website
                                  , LastUpdated = DateTime.Now
                              };
            return company;
        }
    }
}
