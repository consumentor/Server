using System.Collections.Generic;
using System.Linq;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    public class CountryApplicationService : ICountryApplicationService
    {
        private readonly IRepository<Country> _countryRepository;

        public CountryApplicationService(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }
        public Country FindCountry(string countryName, bool onlyPublishedAdvices)
        {
            var result = _countryRepository.Find(c => c.CountryCode.Name == countryName).FirstOrDefault();

            if (result != null && onlyPublishedAdvices)
            {
                result.CountryAdvices = result.CountryAdvices.Where(a => a.Published).ToList();
            }

            return result;
        }

        public IList<Country> FindCountries(string countryName, bool onlyPublishedAdvices)
        {
            var result = _countryRepository.Find(c => c.CountryCode.Name.Equals(countryName) ||
                                                      c.CountryCode.Name.StartsWith(countryName + " ") ||
                                                      c.CountryCode.Name.Contains(" " + countryName + " ") ||
                                                      c.CountryCode.Name.EndsWith(" " + countryName)).ToList();

            if (onlyPublishedAdvices)
            {
                foreach (var country in result)
                {
                    country.CountryAdvices = country.CountryAdvices.Where(a => a.Published).ToList();
                }
            }

            return result;
        }

        public IList<Country> GetAllCountries()
        {
            return _countryRepository.Find(x => x != null).ToList();
        }

        public Country GetCountry(int id)
        {
            return _countryRepository.FindOne(x => x.Id == id);
        }
    }
}
