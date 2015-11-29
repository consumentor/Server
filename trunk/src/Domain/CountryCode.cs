using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "CountryCodes")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class CountryCode
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        protected int Id { get; set; }

        [Column(DbType = "NVarChar(255)")]
        [DataMember(Name = "countryName")]
        public string Name { get; set; }

        /// <summary>
        /// Below is a complete list of the current officially assigned ISO 3166-1 numeric codes, 
        /// with country names being English short country names officially used by the ISO 3166 Maintenance Agency (ISO 3166/MA):
        /// </summary>
        [Column(DbType = "NVarChar(255)")]
        [DataMember(Name = "isoCode")]
        public int ISOCode { get; set; }

        [Column(DbType = "NVarChar(50)")]
        [DataMember(Name = "gs1PrefixCode")]
        public string GS1PrefixCode { get; set; }
    }
}
