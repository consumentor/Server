using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Server
{
    public class CertificationMarkDomainService : ICertificationMarkDomainService
    {
        private readonly IRepository<CertificationMark> _certificationMarkRepository;
        private readonly RepositoryFactory _repositoryFactory;

        public CertificationMarkDomainService(IRepository<CertificationMark> certificationMarkRepository, RepositoryFactory repositoryFactory)
        {
            _certificationMarkRepository = certificationMarkRepository;
            _repositoryFactory = repositoryFactory;
        }

        public CertificationMark CreateCertificationMark(Mentor certifier, CertificationMark certificationMarkToCreate)
        {
            var newCertificationMark = new CertificationMark();
            newCertificationMark.CopyStringProperties(certificationMarkToCreate);
            SetCertifier(newCertificationMark, certifier);

            _certificationMarkRepository.Add(newCertificationMark);
            _certificationMarkRepository.Persist();

            return newCertificationMark;
        }

        public CertificationMark GetCertificationMarkById(int id)
        {
            try
            {
                return _certificationMarkRepository.FindOne(x => x.Id == id);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public IList<CertificationMark> GetAllCertificationMarks()
        {
            return _certificationMarkRepository.Find(x => x != null).ToList();
        }

        public CertificationMark FindCertificationMark(string certificationName)
        {
            return _certificationMarkRepository.Find(x => x.CertificationName == certificationName).FirstOrDefault();
        }

        public CertificationMark UpdateCertificationMark(CertificationMark updatedCertificationMark)
        {
            var certificationMarkToUpdate =
                _certificationMarkRepository.FindOne(x => x.Id == updatedCertificationMark.Id);
            certificationMarkToUpdate.CopyStringProperties(updatedCertificationMark);
            _certificationMarkRepository.Persist();

            return certificationMarkToUpdate;
        }

        public void DeleteCertificationMark(CertificationMark certificationMarkToDelete)
        {
            certificationMarkToDelete = _certificationMarkRepository.FindOne(x => x.Id == certificationMarkToDelete.Id);
            _certificationMarkRepository.Delete(certificationMarkToDelete);
            _certificationMarkRepository.Persist();
        }

        public IList<CertificationMark> GetCertificationMarksByCertifier(Mentor mentor)
        {
            var certificationMarks = _certificationMarkRepository.Find(x => x.MentorId == mentor.Id);
            return certificationMarks.ToList();
        }

        public void AddOpvCertificationMarkMapping(OpvCertificationMarkMapping opvCertificationMarkMapping)
        {
            using (var opvCertificationMarkMappingRepository = _repositoryFactory.Build<IRepository<OpvCertificationMarkMapping>, OpvCertificationMarkMapping>())
            {
                opvCertificationMarkMappingRepository.Add(opvCertificationMarkMapping);
                opvCertificationMarkMappingRepository.Persist();
            }
        }

        public IList<OpvCertificationMarkMapping> GetOpvCertificationMarkMappings()
        {
            using (var opvCertificationMarkMappingRepository = _repositoryFactory.Build<IRepository<OpvCertificationMarkMapping>, OpvCertificationMarkMapping>())
            {
                var mappings =
                    opvCertificationMarkMappingRepository.Find(x => x != null).ToList();
                return mappings.Select(opvCertificationMarkMapping => _certificationMarkRepository.FindDomainObject(opvCertificationMarkMapping)).ToList();
            }
        }

        public void DeleteCertificationMarkMapping(int id)
        {
            using (var opvCertificationMarkMappingRepository = _repositoryFactory.Build<IRepository<OpvCertificationMarkMapping>, OpvCertificationMarkMapping>())
            {
                var mappingToDelete = opvCertificationMarkMappingRepository.FindOne(x => x.Id == id);
                opvCertificationMarkMappingRepository.Delete(mappingToDelete);
                opvCertificationMarkMappingRepository.Persist();
            }
        }

        private void SetCertifier(CertificationMark newCertificationMark, Mentor certifier)
        {
            var certififer = _certificationMarkRepository.FindDomainObject(certifier);
            newCertificationMark.Certifier = certififer;
        }
    }
}
