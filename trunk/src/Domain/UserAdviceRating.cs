using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "UserAdviceRatings")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class UserAdviceRating
    {

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [Column(DbType = "int")]
        [DataMember(Name = "rating")]
        public int Rating { get; set; }

        [Column(CanBeNull = false)]
        [DataMember(Name = "userId")]
        public int UserId { get; set; }
        private EntityRef<User> _entityRefUser = default(EntityRef<User>);
        [Association(ThisKey = "UserId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefUser")]

        public User User
        {
            get { return _entityRefUser.Entity; }
            set
            {
                if (value != null)
                {
                    UserId = value.Id;
                }
                _entityRefUser.Entity = value;
            }
        }

        [Column(CanBeNull = false)]
        [DataMember(Name = "adviceId")]
        public int AdviceId { get; set; }
        private EntityRef<AdviceBase> _entityRefAdviceBase = default(EntityRef<AdviceBase>);
        [Association(ThisKey = "AdviceId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefAdviceBase")]

        public AdviceBase Advice
        {
            get { return _entityRefAdviceBase.Entity; }
            set
            {
                if (value != null && value.Id.HasValue)
                {
                    AdviceId = value.Id.Value;
                }
                _entityRefAdviceBase.Entity = value;
            }
        }

        [Column(DbType = "varchar(50)")]
        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }

    }
}
