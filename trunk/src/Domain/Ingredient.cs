using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Ingredients")]
    [DataContract(Namespace = Base.DataContractNamespace)]    
    public class Ingredient : IAdviceable<IngredientAdvice>
    {
        public Ingredient()
        {
            this.LastUpdated = DateTime.Now;
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetIngredientAdvices = new EntitySet<IngredientAdvice>(onAdd => onAdd.Ingredient = this,
                                                                          onRemove => { onRemove.Ingredient = null; });
            _entitySetIngridientStatistics = new EntitySet<IngredientStatistic>(onAdd => onAdd.Ingredient = this,
                                                                                onRemove =>
                                                                                    { onRemove.Ingredient = null; });
            _entitySetProductIngredients = new EntitySet<ProductIngredient>(onAdd => onAdd.Ingredient = this,
                                                                            onRemove => { onRemove.Ingredient = null; });
            _entitySetAlternativeIngredientName =
                new EntitySet<AlternativeIngredientName>(onAdd => onAdd.Ingredient = this,
                                                         onRemove => { onRemove.Ingredient = null; });
        }

        [OnDeserializing]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Parameter 'context' of 'DomainObject.OnDeserializing(StreamingContext)' is never used. Remove the parameter or use it in the method body.")]
        private void OnDeserializing(StreamingContext context)
        {
            SetupEntitySets();
        }

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DisplayName("Ingredient name")]
        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember(Name = "ingredientName")]
        public string IngredientName { get; set; }

        private EntitySet<IngredientAdvice> _entitySetIngredientAdvices;
        [Association(ThisKey = "Id", OtherKey = "IngredientsId", Storage = "_entitySetIngredientAdvices")]
        [DataMember(Name = "ingredientAdvices")]
        public IList<IngredientAdvice> IngredientAdvices
        {
            get { return _entitySetIngredientAdvices; }
            set { _entitySetIngredientAdvices.Assign(value); }
        }

        private EntitySet<IngredientStatistic> _entitySetIngridientStatistics;
        [Association(ThisKey = "Id", OtherKey = "IngredientId", Storage = "_entitySetIngridientStatistics")]
        public IList<IngredientStatistic> IngredientStatistics
        {
            get { return _entitySetIngridientStatistics; }
            set { _entitySetIngridientStatistics.Assign(value); } 
        }

        [Column]
        public int? ParentId { get; set; }
        private EntityRef<Ingredient> _entityRefParent = default(EntityRef<Ingredient>);

        [Association(ThisKey = "ParentId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefParent")]
        public Ingredient Parent
        {
            get { return _entityRefParent.Entity; }
            set { _entityRefParent.Entity = value; }
        }

        [Column]
        [DataMember(Name = "lastUpdated")]
        public DateTime LastUpdated { get; set; }

        public void AddAdvice(IngredientAdvice advice)
        {
            _entitySetIngredientAdvices.Add(advice);
        }

        public void RemoveAdvice(IngredientAdvice adviceToRemove)
        {
            _entitySetIngredientAdvices.Remove(adviceToRemove);
        }

        private EntitySet<ProductIngredient> _entitySetProductIngredients;
        [Association(ThisKey = "Id", OtherKey = "IngredientId", Storage = "_entitySetProductIngredients")]
        internal IList<ProductIngredient> ProductCertificationMarks
        {
            get { return _entitySetProductIngredients; }
            set { _entitySetProductIngredients.Assign(value); }
        }

        private EntitySet<AlternativeIngredientName> _entitySetAlternativeIngredientName;
        [Association(ThisKey = "Id", OtherKey = "IngredientId", Storage = "_entitySetAlternativeIngredientName")]
        public IList<AlternativeIngredientName> EntitySetAlternativeIngredientName
        {
            get { return _entitySetAlternativeIngredientName; }
            set { _entitySetAlternativeIngredientName.Assign(value); }
        }

        //private IList<string> _alternativeNames;
        [DataMember(Name = "alternativeIngredientNames")]
        public IList<string> AlternativeIngredientNames
        {
            get
            {
                return _entitySetAlternativeIngredientName.Select(x => x.AlternativeName).OrderBy(x => x).ToList();
                //if (_alternativeNames == null)
                //{
                //    var moep = _entitySetAlternativeIngredientName.Select(x => x.AlternativeName).ToList();
                //    _alternativeNames = moep;
                //}
                //return _alternativeNames;
            }
        }

    }
}
