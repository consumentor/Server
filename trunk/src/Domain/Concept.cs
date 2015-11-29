using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Concepts")]
    [DataContract(Namespace = Base.DataContractNamespace, IsReference = true)]        
    public class Concept : IAdviceable<ConceptAdvice>
    {
        public Concept()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetConceptAdvices = new EntitySet<ConceptAdvice>(onAdd => onAdd.Concept = this, onRemove => { onRemove.Concept = null; });
            _entitySetConceptStatistics = new EntitySet<ConceptStatistic>(onAdd => onAdd.Concept = this, onRemove => { onRemove.Concept = null; });
        }

        [OnDeserializing]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Parameter 'context' of 'DomainObject.OnDeserializing(StreamingContext)' is never used. Remove the parameter or use it in the method body.")]
        private void OnDeserializing(StreamingContext context)
        {
            SetupEntitySets();
        }        
        
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember(Name = "conceptTerm")]
        public string ConceptTerm { get; set; }

        private EntitySet<ConceptAdvice> _entitySetConceptAdvices;
        [Association(ThisKey = "Id", OtherKey = "ConceptsId", Storage = "_entitySetConceptAdvices")]
        [DataMember(Name = "conceptAdvices")]
        public IList<ConceptAdvice> ConceptAdvices
        {
            get { return _entitySetConceptAdvices; }
            set { _entitySetConceptAdvices.Assign(value); }
        }

        private EntitySet<ConceptStatistic> _entitySetConceptStatistics;
        [Association(ThisKey = "Id", OtherKey = "ConceptId", Storage = "_entitySetConceptStatistics")]
        public IList<ConceptStatistic> ConceptStatistics
        {
            get { return _entitySetConceptStatistics; }
            set { _entitySetConceptStatistics.Assign(value); }
        }

        [Column]
        [DataMember(Name = "lastUpdated")]
        public DateTime LastUpdated { get; set; }

        public void AddAdvice(ConceptAdvice advice)
        {
            _entitySetConceptAdvices.Add(advice);
        }

        public void RemoveAdvice(ConceptAdvice adviceToRemove)
        {
            _entitySetConceptAdvices.Remove(adviceToRemove);
        }
    }
}
