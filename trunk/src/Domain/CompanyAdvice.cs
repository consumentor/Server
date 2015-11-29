using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    public class CompanyAdvice : AdviceBase
    {
        [Column(CanBeNull = true)]
        public int? CompanysId { get; set; }
        private EntityRef<Company> _entityRefCompany = default(EntityRef<Company>);

        [Association(ThisKey = "CompanysId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCompany")]
        internal Company Company
        {
            get { return _entityRefCompany.Entity; }
            set { _entityRefCompany.Entity = value; }
        }

        public override string ItemName
        {
            get { return Company.CompanyName; }
            internal set { throw new NotImplementedException(); }
        }

        protected override object CreateInstanceOfSameTypeAsThis()
        {
            return new CompanyAdvice();
        }
    }
}
