using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Tips")]
    [DataContract(Namespace = Base.DataContractNamespace)]   
    public class Tip
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [DataMember]
        [Column(DbType = "varchar(400)", Name = "Tip")]
        public string TipText { get; set; }

        [Column]
        public DateTime LastUpdated { get; set; }

        [Column]
        public bool Published { get; set; }
    }
}
