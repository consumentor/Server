using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class AdviceRequestStatistic : StatisticsBase
    {
        [Column(CanBeNull = true)]
        protected int? AdviceId { get; set; }

        private EntityRef<AdviceBase> _entityRefAdvice = default(EntityRef<AdviceBase>);

        [Association(ThisKey = "AdviceId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefAdvice")]
        internal AdviceBase Advice
        {
            get { return _entityRefAdvice.Entity; }
            set { _entityRefAdvice.Entity = value; }
        }
    }
}
