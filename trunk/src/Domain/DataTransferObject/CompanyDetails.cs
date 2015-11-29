using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Consumentor.ShopGun.Domain.DataTransferObject
{
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class CompanyDetails
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember(Name = "urlToHomePage")]
        public string Url { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember]
        public string PostCode { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public bool IsMember { get; set; }

        [DataMember]
        public string ImageUrlMedium { get; set; }

        [DataMember]
        public string ImageUrlLarge { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }
    }
}
