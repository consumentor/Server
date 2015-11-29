using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "CategoryInfo")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class CategoryInfo
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        public string CategoryName { get; set; }

        [Column(DbType = "NVarChar(4000)")]
        public string InfoText { get; set; }
    }
}
