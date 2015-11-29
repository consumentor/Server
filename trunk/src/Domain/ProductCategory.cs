using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "ProductCategories")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class ProductCategory
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "NVARCHAR(50)")]
        [DataMember(Name = "categoryName")]
        public string CategoryName { get; set; }
    }
}
