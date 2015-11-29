using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class BrandStatistic : StatisticsBase 
    {
        [Column(CanBeNull = true)]
        protected int? BrandId { get; set; }

        private EntityRef<Brand> _entityRefBrand = default(EntityRef<Brand>);

        [Association(ThisKey = "BrandId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefBrand")]
        internal Brand Brand
        {
            get { return _entityRefBrand.Entity; }
            set { _entityRefBrand.Entity = value; }
        }
    }
}
