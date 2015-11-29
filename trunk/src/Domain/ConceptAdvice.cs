using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class ConceptAdvice : AdviceBase
    {
        [Column(CanBeNull = true)]
        public int? ConceptsId { get; set; }
        private EntityRef<Concept> _entityRefConcept = default(EntityRef<Concept>);

        [Association(ThisKey = "ConceptsId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefConcept")]
        internal Concept Concept
        {
            get { return _entityRefConcept.Entity; }
            set { _entityRefConcept.Entity = value; }
        }

        public override string ItemName
        {
            get { return Concept.ConceptTerm; }
            internal set { throw new NotImplementedException(); }
        }

        protected override object CreateInstanceOfSameTypeAsThis()
        {
            return new ConceptAdvice();
        }
    }
}
