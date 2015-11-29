using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Mentors")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Mentor
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "NVarChar(255) not null", CanBeNull = false)]
        [DataMember(Name = "mentorName")]
        public string MentorName { get; set; }

        [Column]
        [DataMember]
        [UIHint("Html")]
        public string Description { get; set; }

        [Column]
        [DataMember]
        public string Url { get; set; }


        [Column]
        [DataMember(Name = "logotypeUrl")]
        public string LogotypeUrl { get; set; }

        [Column]
        [DataMember(Name = "isActive")]
        public bool IsActive { get; set; }
    }
}