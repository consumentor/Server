

using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "UsersInRoles")]
    public class UsersInRole
    {
        [Column(DbType = DatabaseType.IntIdentity, IsDbGenerated = true, IsPrimaryKey = true)]
        protected int Id { get; set; }

        [Column(CanBeNull = false)]
        protected int? UserId { get; set; }
       
        private EntityRef<User> _entityRefUser = default(EntityRef<User>);
        [Association(ThisKey = "UserId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefUser")]

        public User User
        {
            get { return _entityRefUser.Entity; }
            set { _entityRefUser.Entity = value; }
        } 
        
        //[Column(DbType = "int not null", IsDbGenerated = false, IsPrimaryKey= true)]
        //public string UserId { get; set; }

        [Column(CanBeNull = false)]
        protected int? RoleId { get; set; }

        private EntityRef<Role> _entityRefRole = default(EntityRef<Role>);
        [Association(ThisKey = "RoleId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefRole")]

        public Role Role
        {
            get { return _entityRefRole.Entity; }
            set { _entityRefRole.Entity = value; }
        } 
    }  
}