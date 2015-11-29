using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server.Interfaces
{
    public interface ICategoryInfoDomainService
    {
        CategoryInfo CreateCategoryInfo(CategoryInfo categoryInfoToCreate);
        CategoryInfo GetCategoryInfo(int categoryInfoId);
        IList<CategoryInfo> GetAllCategoryInfos();
        CategoryInfo GetRandomCategoryInfo(string categoryName);
        CategoryInfo UpdateCategoryInfo(CategoryInfo updatedCategoryInfo);
        bool DeleteCategoryInfo(int categoryInfoId);
    }
}