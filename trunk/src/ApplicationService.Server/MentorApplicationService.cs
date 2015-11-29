using System.Collections.Generic;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    public class MentorApplicationService : IMentorApplicationService
    {
        private readonly IMentorDomainService _mentorDomainService;

        public MentorApplicationService(IMentorDomainService mentorDomainService)
        {
            _mentorDomainService = mentorDomainService;
        }

        public Mentor GetMentorById(int id)
        {
            return _mentorDomainService.GetMentorById(id);
        }

        public IList<Mentor> GetAllMentors()
        {
            return _mentorDomainService.GetAllMentors();
        }
    }
}
