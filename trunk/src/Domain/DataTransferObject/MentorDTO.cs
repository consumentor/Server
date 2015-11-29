using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain.DataTransferObject
{
    public class MentorDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string MentorName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}
