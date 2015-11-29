using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface ICountryApplicationService
    {
        Country FindCountry(string countryName, bool onlyPublishedAdvices);
        IList<Country> FindCountries(string countryName, bool onlyPublishedAdvices);
        IList<Country> GetAllCountries();
        Country GetCountry(int id);
    }
}
