using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class CountryStatistic : StatisticsBase
    {
        [Column(CanBeNull = true)]
        protected int? CountryId { get; set; }

        private EntityRef<Country> _entityRefCountry =  default(EntityRef<Country>);

        [Association(ThisKey = "CountryId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCountry")]
        internal Country Country
        {
            get { return _entityRefCountry.Entity; }
            set { _entityRefCountry.Entity = value; }
        }
    }
}
