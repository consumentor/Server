using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class ConceptApplicationService : IConceptApplicationService
    {
        private readonly IRepository<Concept> _conceptRepository;
        public ILogger Log { get; set; }

        public ConceptApplicationService(IRepository<Concept> conceptRepository)
        {
            _conceptRepository = conceptRepository;
        }

        public Concept GetConcept(int conceptId)
        {
            return _conceptRepository.FindOne(c => c.Id == conceptId);
        }

        public IList<Concept> GetAllConcepts()
        {
            return _conceptRepository.Find(c => c != null).OrderBy(c => c.ConceptTerm).ToList();
        }

        public Concept FindConcept(string conceptName, bool onlyPublishedAdivices)
        {
            var result = _conceptRepository.Find(c => c.ConceptTerm == conceptName).FirstOrDefault();

            if (result != null && onlyPublishedAdivices)
            {
                result.ConceptAdvices = result.ConceptAdvices.Where(a => a.Published).ToList();
            }

            return result;
        }

        public IList<Concept> FindConcepts(string conceptName, bool onlyPublishedAdvices)
        {
            var result = _conceptRepository.Find(c => c.ConceptTerm.Equals(conceptName) ||
                                                      c.ConceptTerm.StartsWith(conceptName + " ") ||
                                                      c.ConceptTerm.Contains(" " + conceptName + " ") ||
                                                      c.ConceptTerm.EndsWith(" " + conceptName)).ToList();

            if (onlyPublishedAdvices)
            {
                foreach (var concept in result)
                {
                    concept.ConceptAdvices = concept.ConceptAdvices.Where(a => a.Published).ToList();
                }
            }

            return result;
        }

        public Concept CreateConcept(Concept conceptToCreate)
        {
            conceptToCreate.LastUpdated = DateTime.Now;
            _conceptRepository.Add(conceptToCreate);
            _conceptRepository.Persist();
            Log.Debug("Concept added");
            return conceptToCreate;
        }

        public Concept UpdateConcept(Concept updatedConcept)
        {
            var conceptToUpdate = _conceptRepository.FindOne(c => c.Id == updatedConcept.Id);
            conceptToUpdate.ConceptTerm = updatedConcept.ConceptTerm;
            conceptToUpdate.LastUpdated = DateTime.Now;
            _conceptRepository.Persist();

            return conceptToUpdate;
        }
    }
}
