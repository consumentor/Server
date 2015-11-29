using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class CompanyApplicationService : ICompanyApplicationService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IRepository<Company> _companyRepository;

        public CompanyApplicationService(RepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _companyRepository = _repositoryFactory.Build<IRepository<Company>, Company>();
        }

        public ILogger Log { get; set; }

        public Company FindCompany(string companyName, bool onlyPublisheAdvices)
        {
            var result = _companyRepository.Find(c => c.CompanyName == companyName).FirstOrDefault();

            if (result != null && onlyPublisheAdvices)
            {
                result.CompanyAdvices = result.CompanyAdvices.Where(a => a.Published).ToList();
            }

            return result;
        }

        public IList<Company> FindCompanies(string companyName, bool onlyPublishedAdvices)
        {
            var result = _companyRepository.Find(c => c.CompanyName.Equals(companyName) ||
                                                      c.CompanyName.StartsWith(companyName + " ") ||
                                                      c.CompanyName.Contains(" " + companyName + " ") ||
                                                      c.CompanyName.EndsWith(" " + companyName)).ToList();

            if (onlyPublishedAdvices)
            {
                foreach (var company in result)
                {
                    company.CompanyAdvices = company.CompanyAdvices.Where(a => a.Published).ToList();
                }
            }

            return result;
        }

        public Company GetCompany(int companyId)
        {
            return _companyRepository.FindOne(c => c.Id == companyId);
        }

        public Company GetCompany(int companyId, bool onlyPublishedAdvices)
        {
            var company = _companyRepository.FindOne(c => c.Id == companyId);

            if (onlyPublishedAdvices)
            {
                    company.CompanyAdvices = company.CompanyAdvices.Where(a => a.Published).ToList();
             
            }
            return company;
        }

        public IList<Company> GetAllCompanies()
        {
            return _companyRepository.Find(c => c != null).OrderBy(x => x.CompanyName).ToList();
        }

        public Company CreateCompany(Company companyToCreate)
        {
            companyToCreate.LastUpdated = DateTime.Now;

            SetOwner(companyToCreate, companyToCreate.ParentId);
            SetCountry(companyToCreate, companyToCreate.CountryId);

            _companyRepository.Add(companyToCreate);
            _companyRepository.MergePersist();
            Log.Debug("Company created");
            return companyToCreate;
        }

        public Company UpdateCompany(Company updatedCompany)
        {
            var companyToUpdate = _companyRepository.FindOne(x => x.Id == updatedCompany.Id);
            SetOwner(companyToUpdate, updatedCompany.ParentId);

            PropertyInfo[] properties = typeof(Company).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                // Only work with strings
                if (p.PropertyType != typeof(string))
                {
                    continue;
                }

                // If not writable then cannot null it; if not readable then cannot check it's value
                if (!p.CanWrite || !p.CanRead)
                {
                    continue;
                }

                MethodInfo mget = p.GetGetMethod(false);
                MethodInfo mset = p.GetSetMethod(false);

                // Get and set methods have to be public
                if (mget == null)
                {
                    continue;
                }
                if (mset == null)
                {
                    continue;
                }

                p.SetValue(companyToUpdate, p.GetValue(updatedCompany, null), null);
            }

            companyToUpdate.LastUpdated = DateTime.Now;
            _companyRepository.Persist();
            return companyToUpdate;
        }

        /// <summary>
        /// Only deletes company if it does not have any references to other domain objects (e.g. advices)
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public bool DeleteCompany(int companyId)
        {
            var companyToDelete = _companyRepository.FindOne(x => x.Id == companyId);
            try
            {
                _companyRepository.Delete(companyToDelete);
                _companyRepository.Persist();
                Log.Debug(string.Format("Company {0} with Id {1} deleted.", companyToDelete.CompanyName, companyToDelete.Id));
                return true;
            }
            catch (Exception e)
            {
                Log.Debug(string.Format("Failed to delete company {0} with Id {1}.", companyToDelete.CompanyName, companyToDelete.Id), e);
                throw;
            }
        }

        public bool DeleteCompany(int companyToDeleteId, int? substitutingCompanyId)
        {
            if (substitutingCompanyId == null)
            {
                return DeleteCompany(companyToDeleteId);
            }
            var companyToDelete = _companyRepository.FindOne(x => x.Id == companyToDeleteId);
            var substitutingCompany = _companyRepository.FindOne(x => x.Id == substitutingCompanyId);

            using (var brandRepository = _repositoryFactory.Build<IRepository<Brand>, Brand>())
            {
                var brands = from brand in brandRepository.Find(x => x.CompanyId == companyToDelete.Id)
                             select brand;
                foreach (var brand in brands)
                {
                    var tempBrand = _companyRepository.FindDomainObject(brand);
                    tempBrand.Owner = substitutingCompany;
                }
            }

            foreach (var companyAdvice in companyToDelete.CompanyAdvices)
            {
                substitutingCompany.AddAdvice(companyAdvice);
            }
            foreach (var companyStatistic in companyToDelete.CompanyStatistics)
            {
                substitutingCompany.CompanyStatistics.Add(companyStatistic);
            }
            var childCompanies = GetChildCompanies(companyToDelete);
            foreach (var childCompany in childCompanies)
            {
                childCompany.Owner = substitutingCompany;
            }

            _companyRepository.Delete(companyToDelete);
            _companyRepository.Persist();
            Log.Debug("Company '{0}' with Id '{1}' deleted. Substitute company: '{2}' - {3}", companyToDelete.CompanyName,
                      companyToDelete.Id, substitutingCompany.Id, companyToDelete.CompanyName);
            return true;
        }

        /// <summary>
        /// Deletes a company and moves all references (e.g. advices) to <paramref name="substitutingCompany"/>.
        /// If <paramref name="substitutingCompany"/> is <code>null</code> all references will be deleted as well!
        /// </summary>
        /// <param name="companyToDelete"></param>
        /// <param name="substitutingCompany"></param>
        /// <returns></returns>
        public bool DeleteCompany(Company companyToDelete, Company substitutingCompany)
        {
            var substitutingCompanyId = substitutingCompany == null ? 0 : substitutingCompany.Id;
            return DeleteCompany(companyToDelete.Id, substitutingCompanyId);
        }

        public IList<Company> GetChildCompanies(Company company)
        {
            var childCompanies = _companyRepository.Find(x => x.ParentId == company.Id);
            return childCompanies.ToList();
        }

        private void SetCountry(Company companyToCreate, int? countryId)
        {
            if (countryId != null)
            {
                using (var countryRepository = _repositoryFactory.Build<IRepository<Country>, Country>())
                {
                    var country = countryRepository.FindOne(x => x.Id == countryId);
                    companyToCreate.Country = _companyRepository.FindDomainObject(country);
                }
            }
            else
            {
                companyToCreate.Country = null;
            }
        }

        private void SetOwner(Company company, int? parentCompanyId)
        {
            if (company.Id == parentCompanyId)
            {
                throw new ArgumentException("Company owner must not be the same as actual company!");
            }
            company.Owner = parentCompanyId == null ? null : _companyRepository.FindOne(c => c.Id == parentCompanyId);
        }
    }
}
