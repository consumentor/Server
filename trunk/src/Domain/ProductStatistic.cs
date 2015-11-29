using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace Consumentor.ShopGun.Domain
{
    public class ProductStatistic : StatisticsBase
    {
        [Column(CanBeNull = true)]
        protected int? ProductId { get; set; }

        private EntityRef<Product> _entityRefProduct = default(EntityRef<Product>);

        [Association(ThisKey = "ProductId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefProduct")]
        internal Product Product
        {
            get { return _entityRefProduct.Entity; }
            set { _entityRefProduct.Entity = value; }
        }
    }
}
