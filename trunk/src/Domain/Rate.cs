using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Rates")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Rate
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        protected int Id { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember]
        public string Name { get; set; }

        [Column(CanBeNull = false)]
        [DataMember]
        public int Value { get; set; }
    }
}