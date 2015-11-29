using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using System.Data.Linq;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "SearchStatistics")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class SearchStatistic
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false, DbType = DatabaseType.IntIdentity)]
        protected int Id { get; set; }

        [Column(CanBeNull = false, DbType = DatabaseType.Int)]
        protected int SearchtermId { get; set; }

        private EntityRef<Searchterm> _entityRefSearchterm = default(EntityRef<Searchterm>);
        [Association(ThisKey = "SearchtermId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefSearchterm")]
        internal Searchterm Searchterm
        {
            get { return _entityRefSearchterm.Entity; }
            set { _entityRefSearchterm.Entity = value; }
        }

        [Column(CanBeNull = true)]
        protected int? UserId { get; set; }

        private EntityRef<User> _entityRefUser = default(EntityRef<User>);
        [Association(ThisKey = "UserId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefUser")]
        [DataMember(Name = "user")]
        public User User
        {
            get { return _entityRefUser.Entity; }
            set { _entityRefUser.Entity = value; }
        }

        [Column(CanBeNull = false, DbType = DatabaseType.Datetime2)]
        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; set; }

        [Column(CanBeNull = true)]
        public string UserAgent { get; set; }

        [Column(CanBeNull = true)]
        public string Imei{ get; set; }

        [Column(CanBeNull = true)]
        public string Model { get; set; }

        [Column(CanBeNull = false)]
        public bool ResultFound { get; set; }

        [Column(CanBeNull = true)]
        public string OsVersion { get; set; }
    }
}
