using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IMentorApplicationService
    {
        Mentor GetMentorById(int id);
        IList<Mentor> GetAllMentors();
    }
}