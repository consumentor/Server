using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Semaphore")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Semaphore
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember(Name = "colorName")]
        public string ColorName { get; set; }

        [Column(CanBeNull = false)]
        [DataMember(Name = "value")]
        public int Value { get; set; }
    }
}