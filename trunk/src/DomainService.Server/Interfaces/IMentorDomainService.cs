using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server.Interfaces
{
    public interface IMentorDomainService
    {
        IList<Mentor> GetAllMentors();
        Mentor GetMentorById(int id);
        Mentor CreateNewMentor(string mentorName);
        Mentor UpdateMentor(Mentor updatedMentor);
        void DeleteMentor(Mentor mentorToDelete);
    }
}
