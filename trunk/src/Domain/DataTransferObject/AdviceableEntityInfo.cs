using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain.DataTransferObject
{
    [DataContract(Namespace = Base.DataContractNamespace)]
    public abstract class AdviceableEntityInfo
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<AdviceInfo> AdviceInfos { get; set; }

        [DataMember]
        public Dictionary<string, Dictionary<string, int>> AdviceCountBySemaphoreValueByAdviceTagId
        {
            get
            {
                var groupedByTag = AdviceInfos.GroupBy(x => x.AdviceTag);
                var moep = groupedByTag
                    .ToDictionary(tag => tag.Key, adviceCountBySemaphore =>adviceCountBySemaphore.GroupBy(z => z.Semaphore)
                        .ToDictionary(semaphore => semaphore.Key, count => count.Count()));
                return moep;
            }
            set { }
        }

    }
}
