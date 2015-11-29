using System;
using System.Collections.Generic;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Server
{
    public class CategoryInfoDomainService : ICategoryInfoDomainService
    {
        private readonly IRepository<CategoryInfo> _categoryInfoRepository;

        public CategoryInfoDomainService(IRepository<CategoryInfo> categoryInfoRepository)
        {
            _categoryInfoRepository = categoryInfoRepository;
        }

        public CategoryInfo CreateCategoryInfo(CategoryInfo categoryInfoToCreate)
        {
            _categoryInfoRepository.Add(categoryInfoToCreate);
            _categoryInfoRepository.Persist();

            return categoryInfoToCreate;
        }

        public CategoryInfo GetCategoryInfo(int categoryInfoId)
        {
            return _categoryInfoRepository.FindOne(x => x.Id == categoryInfoId);
        }

        public IList<CategoryInfo> GetAllCategoryInfos()
        {
            return _categoryInfoRepository.Find(x => x != null).ToList();
        }

        public CategoryInfo GetRandomCategoryInfo(string categoryName)
        {
            var categoryInfos = _categoryInfoRepository.Find(x => x.CategoryName == categoryName).ToArray();
            if (categoryInfos.Count() > 0)
            {
                var randomizer = new Random(DateTime.Now.Millisecond);
                var choice = randomizer.Next(0, categoryInfos.Count()-1);
                return categoryInfos[choice];
            }
            return null;
        }

        public CategoryInfo UpdateCategoryInfo(CategoryInfo updatedCategoryInfo)
        {
            var categoryInfoToUpdate = _categoryInfoRepository.FindOne(x => x.Id == updatedCategoryInfo.Id);
            categoryInfoToUpdate.CategoryName = updatedCategoryInfo.CategoryName;
            categoryInfoToUpdate.InfoText = updatedCategoryInfo.InfoText;
            _categoryInfoRepository.Persist();
            return updatedCategoryInfo;
        }

        public bool DeleteCategoryInfo(int categoryInfoId)
        {
            var categoryInfoToDelete = _categoryInfoRepository.FindOne(x => x.Id == categoryInfoId);
            _categoryInfoRepository.Delete(categoryInfoToDelete);
            _categoryInfoRepository.Persist();
            return true;
        }
    }
}