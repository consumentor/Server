using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class CompanyStatistic : StatisticsBase
    {
        [Column(CanBeNull = true)]
        protected int? CompanyId { get; set; }

        private EntityRef<Company> _entityRefCompany = default(EntityRef<Company>);

        [Association(ThisKey = "CompanyId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCompany")]
        internal Company Company
        {
            get { return _entityRefCompany.Entity; }
            set { _entityRefCompany.Entity = value; }
        }
    }
}