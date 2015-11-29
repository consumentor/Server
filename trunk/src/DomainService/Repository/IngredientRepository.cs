using System.Data.Linq;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using ShopgunDataContext = Consumentor.ShopGun.Repository.DataContext;

namespace Consumentor.ShopGun.DomainService.Repository
{
    public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
    {
        public IngredientRepository(ShopgunDataContext context)
            : base(context)
        {
        }
        public bool HasChanges
        {
            get
            {
                var changeSet = Context.GetChangeSet();
                return changeSet.Inserts.Count > 0 || changeSet.Updates.Count > 0 || changeSet.Deletes.Count > 0;
            }
        }

        public bool IngredientNameAvailable(string name)
        {
            return !Context.GetTable<Ingredient>().Any(i => i.IngredientName == name) &&
                   !Context.GetTable<AlternativeIngredientName>().Any(ain => ain.AlternativeName == name);
        }

        public void Refresh(Ingredient ingredient)
        {
            //Refresh regular members
            Context.Refresh(RefreshMode.KeepChanges, ingredient);

            //Refresh EntityRefs - Do we need this:Boris E??
            //if (ingredient.Order != null)
            //    Context.Refresh(RefreshMode.KeepChanges, ingredient.Order);

            //Refresh EntitySets
            RefreshIngredientAdvices(ingredient);
        }

        void IIngredientRepository.Delete<T>(T item)
        {
            //if (Context.GetChangeSet().Inserts.Contains(item) || Context.GetChangeSet().Updates.Contains(item))
                Delete(item);
        }


        private void RefreshIngredientAdvices(Ingredient ingredient)
        {
            var ingredientAdvices = (EntitySet<IngredientAdvice>)ingredient.IngredientAdvices;
            //if (ingredientAdvices.HasLoadedOrAssignedValues)
            //{
                var allIngredientAdvices =
                    Context.GetTableForType(typeof (IngredientAdvice)).OfType<IngredientAdvice>().Where(
                        a => a.IngredientsId == ingredient.Id).ToList();

                ingredient.IngredientAdvices.Clear();

            foreach (var ingredientAdvice in allIngredientAdvices)
            {
                ingredient.AddAdvice(ingredientAdvice);
            }

            Context.Refresh(RefreshMode.KeepChanges, ingredient);

                //Refresh EntityRefs);
                foreach (var ingredientAdvice in ingredient.IngredientAdvices)
                {
                    Context.Refresh(RefreshMode.KeepChanges, ingredientAdvice);
                    Context.Refresh(RefreshMode.KeepChanges, ingredientAdvice.Mentor);
                    Context.Refresh(RefreshMode.KeepChanges, ingredientAdvice.Semaphore);
                }
            //}
        }

        //private void RefreshAreas(RollMap rollMap)
        //{
        //    var areas = (EntitySet<Area>)rollMap.Areas;
        //    if (areas.HasLoadedOrAssignedValues)
        //    {
        //        List<Area> allAreas = Context.GetTable<Area>().AsQueryable().Where(a => a.RollMap.Id == rollMap.Id).ToList();
        //        foreach (Area area in allAreas.Except(rollMap.Areas))
        //        {
        //            rollMap.Areas.Add(area);
        //        }
        //        foreach (Area area in rollMap.Areas.Except(allAreas))
        //        {
        //            rollMap.Areas.Remove(area);
        //        }

        //        //Refresh EntityRefs
        //        foreach (Area area in rollMap.Areas)
        //        {
        //            Context.Refresh(RefreshMode.KeepChanges, area);
        //            Context.Refresh(RefreshMode.KeepChanges, area.EndPosition);
        //            Context.Refresh(RefreshMode.KeepChanges, area.StartPosition);
        //        }
        //    }
        //}
    }
}