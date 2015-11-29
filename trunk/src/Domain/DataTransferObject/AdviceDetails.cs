using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain.DataTransferObject
{
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class AdviceDetails : AdviceInfo
    {
        [DataMember]
        public string Advice{ get; set; }
    }
}
