using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.ApplicationService.Server.Interfaces
{
    public interface IBrandApplicationService
    {
        Brand CreateBrand(Brand brandToCreate);
        Brand GetBrand(int brandId);
        Brand GetBrand(int brandId, bool onlyPublishedAdvices);
        IList<Brand> GetAllBrands();
        IList<Brand> GetBrandsWithAdvicesByMentor(int mentorId);
        Brand UpdateBrand(Brand updatedBrand);
        bool DeleteBrand(int brandId);
        bool DeleteBrand(int brandId, int? substitutingBrandId);

        Brand FindBrand(string brandName, bool onlyPublishedAdvices);
        IList<Brand> FindBrands(string brandName, bool onlyPublishedAdvices);
    }
}