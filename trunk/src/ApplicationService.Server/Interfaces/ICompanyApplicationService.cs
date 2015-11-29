using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface ICompanyApplicationService
    {
        Company FindCompany(string companyName, bool onlyPublisheAdvices);
        IList<Company> FindCompanies(string companyName, bool onlyPublishedAdvices);
        Company GetCompany(int companyId);
        Company GetCompany(int companyId, bool onlyPublishedAdvices);
        IList<Company> GetAllCompanies();
        Company CreateCompany(Company companyToCreate);
        Company UpdateCompany(Company updatedCompany);
        bool DeleteCompany(int companyId);
        bool DeleteCompany(int companyToDeleteId, int? substitutingCompanyId);
        bool DeleteCompany(Company companyToDelete, Company substitutingCompany);
        IList<Company> GetChildCompanies(Company company);
    }
}
