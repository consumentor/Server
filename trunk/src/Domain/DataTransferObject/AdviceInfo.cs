using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain.DataTransferObject
{
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class AdviceInfo
    {
        [DataMember]
        public string AdviceType { get; set; }
        [DataMember]
        public int AdviceId { get; set; }
        [DataMember]
        public string AdviceIntroduction { get; set; }
        [DataMember]
        public int AdviceableEntityId { get; set; }
        [DataMember]
        public string AdviceableEntityName { get; set; }
        [DataMember]
        public string Semaphore { get; set; }
        [DataMember]
        public string AdviceTag { get; set; }
        [DataMember]
        public int AdvisorId { get; set; }
        [DataMember]
        public string AdvisorName { get; set; }
    }
}
