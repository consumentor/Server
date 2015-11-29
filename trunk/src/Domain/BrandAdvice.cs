using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class BrandAdvice : AdviceBase
    {
        [Column(CanBeNull = true)]
        public int? BrandsId { get; set; }
        private EntityRef<Brand> _entityRefBrand = default(EntityRef<Brand>);

        [Association(ThisKey = "BrandsId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefBrand")]
        internal Brand Brand
        {
            get { return _entityRefBrand.Entity; }
            set { _entityRefBrand.Entity = value; }
        }

        public override string ItemName
        {
            get { return Brand.BrandName; }
            internal set { throw new NotImplementedException(); }
        }

        protected override object CreateInstanceOfSameTypeAsThis()
        {
            return new BrandAdvice();
        }

        protected override void SetClonedData(AdviceBase clone)
        {
            ((BrandAdvice)clone).BrandsId = BrandsId;
            base.SetClonedData(clone);
        }
    }
}
