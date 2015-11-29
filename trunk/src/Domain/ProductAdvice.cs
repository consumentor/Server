using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class ProductAdvice : AdviceBase
    {
        [Column(CanBeNull = true)]
        public int? ProductsId { get; set; }
        private EntityRef<Product> _entityRefProduct = default(EntityRef<Product>);

        [Association(ThisKey = "ProductsId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefProduct")]
        internal Product Product
        {
            get { return _entityRefProduct.Entity; }
            set { _entityRefProduct.Entity = value; }
        }

        public override string ItemName
        {
            get { return (Product != null ? Product.Brand.BrandName + " " + Product.ProductName : ""); }
            internal set {  }
        }

        protected override object CreateInstanceOfSameTypeAsThis()
        {
            return new ProductAdvice();
        }

        protected override void SetClonedData(AdviceBase clone)
        {
            ((ProductAdvice) clone).ProductsId = ProductsId;
            base.SetClonedData(clone);
        }
    }
}