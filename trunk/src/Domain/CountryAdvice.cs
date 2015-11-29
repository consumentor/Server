using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class CountryAdvice : AdviceBase
    {
        [Column(CanBeNull = true)]
        public int? CountrysId { get; set; }
        private EntityRef<Country> _entityRefCountry = default(EntityRef<Country>);

        [Association(ThisKey = "CountrysId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCountry")]
        internal Country Country
        {
            get { return _entityRefCountry.Entity; }
            set { _entityRefCountry.Entity = value; }
        }

        public override string ItemName
        {
            get { return Country.CountryCode.Name; }
            internal set { throw new NotImplementedException(); }
        }

        protected override object CreateInstanceOfSameTypeAsThis()
        {
            return new CountryAdvice();
        }
    }
}
