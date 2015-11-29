using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain.DataTransferObject
{
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class ProductInfo : AdviceableEntityInfo
    {
        [DataMember]
        public string GTIN { get; set; }
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public string BrandName { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
    }
}
