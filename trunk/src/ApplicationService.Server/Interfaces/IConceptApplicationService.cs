using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IConceptApplicationService
    {
        Concept GetConcept(int conceptId);
        IList<Concept> GetAllConcepts();
        Concept FindConcept(string conceptName, bool onlyPublishedAdivices);
        IList<Concept> FindConcepts(string conceptName, bool onlyPublishedAdvices);
        Concept CreateConcept(Concept conceptToCreate);
        Concept UpdateConcept(Concept updatedConcept);
    }
}
