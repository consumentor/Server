using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web.Services;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.ApplicationService.WebService;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.ShopGun.ApplicationService.Server.WebService
{
    [ServiceBehavior(Namespace = Base.DataContractNamespace)]
    public class MentorWebService : WebServiceBase, IMentorContract
    {
        
        private readonly IMentorDomainService _mentorDomainService;
        private readonly IProductApplicationService _productApplicationService;

        public MentorWebService()
        {
            _mentorDomainService = Container.Resolve<IMentorDomainService>();
            _productApplicationService = Container.Resolve<IProductApplicationService>();
        }

        public List<Mentor> GetMentorsXml()
        {
            try
            {
                return (List<Mentor>) _mentorDomainService.GetAllMentors();
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw;
            }
            
        }

        public List<Mentor> GetMentors()
        {
            try
            {
                return (List<Mentor>) _mentorDomainService.GetAllMentors();
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw;
            }
        }

        public List<Product> GetProductsForMentor(int mentorId)
        {
            try
            {
                throw new NotImplementedException();
                //return (List<Product>) _productApplicationService.GetProductsWithAdvicesByMentorId(mentorId);
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw;
            }
        }
    }
}
