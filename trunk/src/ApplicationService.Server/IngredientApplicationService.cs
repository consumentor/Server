using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService;
using Consumentor.ShopGun.DomainService.Repository;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.ApplicationService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class IngredientApplicationService : IIngredientApplicationService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly RepositoryFactory _repositoryFactory;

        public IngredientApplicationService(RepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _ingredientRepository = _repositoryFactory.Build<IIngredientRepository, Ingredient>();
        }

        public ILogger Log { get; set; }

        #region IIngredientApplicationService Members

        public Ingredient GetIngredientById(int ingredientId)
        {
            return _ingredientRepository.FindOne(x => x.Id == ingredientId);
        }

        /// <summary>
        /// Creates a new Ingredient and returns it. If an Ingredient or an AlternativeIngredientName with the same name already exist, null is returned.
        /// </summary>
        /// <param name="ingredientName"></param>
        /// <returns></returns>
        public Ingredient CreateIngredient(string ingredientName)
        {
            var alternativeNameRepostory =
                _repositoryFactory.Build<IRepository<AlternativeIngredientName>, AlternativeIngredientName>();
            if (string.IsNullOrEmpty(ingredientName)
                || _ingredientRepository.Find(i => i.IngredientName == ingredientName).Any()
                || alternativeNameRepostory.Find(ain => ain.AlternativeName == ingredientName).Any())
            {
                return null;
            }
            var ingredient = new Ingredient { IngredientName = ingredientName };
            _ingredientRepository.Add(ingredient);
            _ingredientRepository.Persist();
            return ingredient;
        }

        public Ingredient GetIngredient(int ingredientId)
        {
            return _ingredientRepository.FindOne(i => i.Id == ingredientId);
        }

        public Ingredient GetIngredient(int ingredientId, bool onlyPublishedAdvices)
        {
            var ingredient = _ingredientRepository.FindOne(i => i.Id == ingredientId);
            if (onlyPublishedAdvices)
            {
                ingredient.IngredientAdvices = ingredient.IngredientAdvices.Where(a => a.Published).ToList();
            }

            return ingredient;
        }

        public IList<Ingredient> GetAllIngredients()
        {
            return _ingredientRepository.Find(i => i != null).OrderBy(i => i.IngredientName).ToList();
        }

        public Ingredient UpdateIngredient(Ingredient updatedIngredient)
        {
            var ingredientToUpdate = _ingredientRepository.FindOne(i => i.Id == updatedIngredient.Id);

            ingredientToUpdate.IngredientName = updatedIngredient.IngredientName;
            SetParentIngredient(ingredientToUpdate, updatedIngredient.ParentId);
            ingredientToUpdate.LastUpdated = DateTime.Now;
            _ingredientRepository.Persist();
            Log.Debug("Ingredient {0} with Id {1} updated", ingredientToUpdate.IngredientName, ingredientToUpdate.Id);
            return ingredientToUpdate;
        }

        public bool AddAlternativeName(int ingredientId, string alternativeName)
        {
            if (_ingredientRepository.IngredientNameAvailable(alternativeName))
            {
                var ingredient = _ingredientRepository.FindOne(i => i.Id == ingredientId);
                ingredient.EntitySetAlternativeIngredientName.Add(new AlternativeIngredientName{AlternativeName = alternativeName});
                _ingredientRepository.Persist();
                return true;
            }
            return false;
        }

        public bool RemoveAlternativeName(int ingredientId, string alternativeName)
        {
            var ingredient = _ingredientRepository.FindOne(i => i.Id == ingredientId);
            var nameToRemove =
                ingredient.EntitySetAlternativeIngredientName.FirstOrDefault(ain => ain.AlternativeName == alternativeName);
            var removed = ingredient.EntitySetAlternativeIngredientName.Remove(nameToRemove);
            _ingredientRepository.Persist();
            var alternativeIngredientNameRepository =
                _repositoryFactory.Build<IRepository<AlternativeIngredientName>, AlternativeIngredientName>();
            alternativeIngredientNameRepository.Delete(alternativeIngredientNameRepository.FindOne(ain => ain.Id == nameToRemove.Id));
            alternativeIngredientNameRepository.Persist();
            return removed;
        }

        private void SetParentIngredient(Ingredient ingredientToUpdate, int? parentId)
        {
            if (ingredientToUpdate.Id == parentId)
            {
                throw new ArgumentException("Ingredient owner must not be the same as actual ingredient!");
            }
            ingredientToUpdate.Parent = parentId == null ? null : _ingredientRepository.FindOne(c => c.Id == parentId);
        }

        public bool DeleteIngredient(int ingredientId)
        {
            try
            {
                var ingredientToDelete = _ingredientRepository.FindOne(x => x.Id == ingredientId);
                _ingredientRepository.Delete(ingredientToDelete);
                _ingredientRepository.Persist();
                Log.Debug("Ingredient '{0}' with Id '{1}' deleted.", ingredientToDelete.IngredientName, ingredientToDelete.Id);
                return true;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Failed to delete ingredient with Id '{0}'", ingredientId), e);
                return false;
            }
        }

        public bool DeleteIngredient(int ingredientId, int substitutingIngredientId)
        {
            if (substitutingIngredientId < 1)
            {
                return DeleteIngredient(ingredientId);
            }
            var ingredientToDelete = _ingredientRepository.FindOne(x => x.Id == ingredientId);
            var substitutingIngredient = _ingredientRepository.FindOne(x => x.Id == substitutingIngredientId);

            foreach (var ingredientAdvice in ingredientToDelete.IngredientAdvices)
            {
                substitutingIngredient.AddAdvice(ingredientAdvice);
            }
            foreach (var ingredientStatistic in ingredientToDelete.IngredientStatistics)
            {
                substitutingIngredient.IngredientStatistics.Add(ingredientStatistic);
            }
            foreach (var alternativeIngredientName in ingredientToDelete.EntitySetAlternativeIngredientName)
            {
                substitutingIngredient.EntitySetAlternativeIngredientName.Add(alternativeIngredientName);
            }

            _ingredientRepository.Delete(ingredientToDelete);
            _ingredientRepository.Persist();
            Log.Debug("Ingredient '{0}' with Id '{1}' deleted. Substitute ingredient: '{2}' - {3}", ingredientToDelete.IngredientName,
                      ingredientToDelete.Id, substitutingIngredient.Id, ingredientToDelete.IngredientName);
            return true;
        }

        public Ingredient GetIngredientByName(string ingredientName)
        {
            return _ingredientRepository.FindOne(i => i.IngredientName == ingredientName);
        }

        public Ingredient FindIngredient(string ingredientName, bool withParenAdvices, bool onlyPublishedAdvices)
        {
            var result = _ingredientRepository.Find(i => i.IngredientName == ingredientName).FirstOrDefault();

            if (result == null)
            {
                var alternativeIngredientNameRepository =
                    _repositoryFactory.Build<IRepository<AlternativeIngredientName>, AlternativeIngredientName>();
                var alternativeIngredientName =
                    alternativeIngredientNameRepository.Find(x => x.AlternativeName == ingredientName).FirstOrDefault();
                if (alternativeIngredientName != null)
                {
                    result = _ingredientRepository.FindOne(x => x.Id == alternativeIngredientName.Ingredient.Id);
                }
            }
            if (result != null)
            {
                if (withParenAdvices)
                {
                    AddParentAdvices(result);
                }
                if (onlyPublishedAdvices)
                {
                    result.IngredientAdvices = result.IngredientAdvices.Where(a => a.Published).ToList();
                }
            }

            return result;
        }

        public IList<Ingredient> FindIngredients(string ingredientName, bool withParentAdvices, bool onlyPublishedAdvices)
        {
            var result = _ingredientRepository.Find(i => i.IngredientName.Equals(ingredientName) ||
                                                         i.IngredientName.StartsWith(ingredientName + " ") ||
                                                         i.IngredientName.Contains(" " + ingredientName + " ") ||
                                                         i.IngredientName.EndsWith(" " + ingredientName)).ToList();

            var alternativeIngredientNameRepository =
                _repositoryFactory.Build<IRepository<AlternativeIngredientName>, AlternativeIngredientName>();

            var alternativeIngredientNames =
                alternativeIngredientNameRepository.Find(altName => altName.AlternativeName.Equals(ingredientName) ||
                                                                    altName.AlternativeName.StartsWith(ingredientName +
                                                                                                       " ") ||
                                                                    altName.AlternativeName.Contains(" " +
                                                                                                     ingredientName +
                                                                                                     " ") ||
                                                                    altName.AlternativeName.EndsWith(" " +
                                                                                                     ingredientName)).
                    ToList();

            //Add ingredients that have been found via alternative name and are not yet in the result list.
            result.AddRange(
                alternativeIngredientNames.Select(x => _ingredientRepository.FindOne(i => i.Id == x.Ingredient.Id)).
                    Except(result));


            foreach (var ingredient in result)
            {
                if (withParentAdvices)
                {
                    AddParentAdvices(ingredient);
                }
                if (onlyPublishedAdvices)
                {
                    ingredient.IngredientAdvices = ingredient.IngredientAdvices.Where(a => a.Published).ToList();
                }
            }

            return result;
        }

        private void AddParentAdvices(Ingredient ingredient)
        {
            if (ingredient.Parent == null)
            {
                return;
            }
            AddParentAdvices(ingredient.Parent);
            var allAdvices = new List<IngredientAdvice>(ingredient.IngredientAdvices);
            allAdvices.AddRange(ingredient.Parent.IngredientAdvices);
            ingredient.IngredientAdvices = allAdvices;
        }

        public IList<Ingredient> GetIngredientsWithAdvicesByMentor(Mentor mentor)
        {
            var result = _ingredientRepository.Find(i => i.IngredientAdvices.Any(a => a.Mentor == mentor)).ToList();

            foreach (var ingredient in result)
            {
                var advicesFromMentor = from a in ingredient.IngredientAdvices
                                        where a.Mentor.Id == mentor.Id
                                        orderby a.Id descending
                                        select a;
                ingredient.IngredientAdvices = advicesFromMentor.ToList();
            }

            return result;
        }

        public Ingredient AddIngredient(string ingredientName)
        {
            try
            {
                var existingIngredient = _ingredientRepository.FindOne(i => i.IngredientName == ingredientName);
                return existingIngredient;
            }
            catch (ArgumentException)
            {
                var newIngredient = new Ingredient
                {
                    IngredientName = ingredientName,
                    LastUpdated = DateTime.Now
                };

                _ingredientRepository.Add(newIngredient);
                _ingredientRepository.Persist();
                return newIngredient;
            }
        }

        /// <summary>
        /// Takes a string with comma seperated ingredient names and returns a list of existing Ingredient objects (only published advices!)
        /// </summary>
        /// <remarks>
        /// Splits tableOfContents string by ',' (comma), ' ' (space) and '(' ')' (parentheses).
        /// </remarks>
        /// <param name="tableOfContents"></param>
        /// <returns></returns>
        public IList<Ingredient> FindIngredientsForTableOfContents(string tableOfContents)
        {
            var ingredientList = new List<Ingredient>();
            if (tableOfContents != null)
            {
                //Todo: Should be solved with regex i guess... /SB
                var toc = tableOfContents.ToLower();
                toc = toc.Replace(",", " ");
                toc = toc.Replace("(", " ");
                toc = toc.Replace(")", " ");
                toc = toc.Replace("*", " ");
                toc = toc.Replace(":", " ");
                toc = toc.Replace("\n", " ");
                toc = toc.TrimEnd('.');
                toc = toc.Replace(".", " ");

                var ingredients = _ingredientRepository.Find(x => x != null);
                var prodIngredients = from i in ingredients
                                      where toc.StartsWith(i.IngredientName.ToLower() + " ")
                                            || toc.Equals(i.IngredientName.ToLower())
                                            || toc.Contains(" " + i.IngredientName.ToLower() + " ")
                                            || toc.EndsWith(" " + i.IngredientName.ToLower())
                                      select i;

                //Don't take duplicate ingredients and only take published advices
                foreach (var ingredient in prodIngredients.Where(ingredient => !ingredientList.Contains(ingredient)))
                {
                    ingredientList.Add(ingredient);
                }

                var alternativeIngredientNameRepository =
                    _repositoryFactory.Build<IRepository<AlternativeIngredientName>, AlternativeIngredientName>();

                var alternativeIngredientNames = alternativeIngredientNameRepository.Find(x => x != null);
                var alternativeIngredients = from i in alternativeIngredientNames
                                             where toc.StartsWith(i.AlternativeName.ToLower() + " ")
                                                   || toc.Equals(i.AlternativeName.ToLower())
                                                   || toc.Contains(" " + i.AlternativeName.ToLower() + " ")
                                                   || toc.EndsWith(" " + i.AlternativeName.ToLower())
                                             select i;

                //Add ingredients that have been found via alternative name and are not yet in the result list.
                ingredientList.AddRange(
                    alternativeIngredients.Select(x => _ingredientRepository.FindOne(i => i.Id == x.Ingredient.Id)).
                        Except(ingredientList));

                foreach (var ingredient in ingredientList)
                {
                    AddParentAdvices(ingredient);
                    ingredient.IngredientAdvices =
                        ingredient.IngredientAdvices.Where(a => a.Published).OrderBy(x => x.Semaphore.Value).ToList();
                }
            }
            return ingredientList;
        }

        #endregion
    }
}
