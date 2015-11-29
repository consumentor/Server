using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server.Interfaces
{
    public interface ICertificationMarkDomainService
    {
        CertificationMark CreateCertificationMark(Mentor certifier, CertificationMark certificationMarkToCreate);
        CertificationMark GetCertificationMarkById(int id);
        IList<CertificationMark> GetAllCertificationMarks();
        CertificationMark FindCertificationMark(string certificationName);
        CertificationMark UpdateCertificationMark(CertificationMark updatedCertificationMark);
        void DeleteCertificationMark(CertificationMark certificationMarkToDelete);
        IList<CertificationMark> GetCertificationMarksByCertifier(Mentor mentor);

        void AddOpvCertificationMarkMapping(OpvCertificationMarkMapping opvCertificationMarkMapping);

        IList<OpvCertificationMarkMapping> GetOpvCertificationMarkMappings();

        void DeleteCertificationMarkMapping(int id);
    }
}