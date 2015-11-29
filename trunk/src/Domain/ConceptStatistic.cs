using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class ConceptStatistic : StatisticsBase
    {
        [Column(CanBeNull = true)]
        protected int? ConceptId { get; set; }

        private EntityRef<Concept> _entityRefConcept = default(EntityRef<Concept>);

        [Association(ThisKey = "ConceptId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefConcept")]
        internal Concept Concept
        {
            get { return _entityRefConcept.Entity; }
            set { _entityRefConcept.Entity = value; }
        }
    }
}
