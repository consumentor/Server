using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;


namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "CSRInfos")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class CSRInfo
    {

        public CSRInfo()
        {
            
        }

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) NOT NULL")]
        [DataMember(Name = "tag")]
        public string Tag { get; set; }

        [Column(DbType = "NVarChar(MAX)", CanBeNull = true)]
        [DataMember(Name = "description")]
        [System.ComponentModel.DataAnnotations.UIHint("Html")]
        public string Description { get; set; }

        [Column(CanBeNull = false)]
        public int? CompanysId { get; set; }
        private EntityRef<Company> _entityRefCompany = default(EntityRef<Company>);
        [Association(ThisKey = "CompanysId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCompany")]

        public Company Company
        {
            get { return _entityRefCompany.Entity; }
            set
            {
                if (value != null)
                {
                    CompanysId = value.Id;
                }
                _entityRefCompany.Entity = value;
            }
        }
    }
}
