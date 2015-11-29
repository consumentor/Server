
using System.Data.Linq.Mapping;

namespace Consumentor.ShopGun.Domain
{
    //[Table(Name = "Roles")]
    //[InheritanceMapping(Code = "EndConsumerRole", Type = typeof(EndConsumerRole), IsDefault = true)]
    //[InheritanceMapping(Code = "AdministratorRole", Type = typeof(AdministratorRole))]    
    //public class Role
    //{
    //    [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
    //    protected int Id { get; set; }

    //    [Column(CanBeNull = false)]
    //    protected int? UserId { get; set; }
    //    private EntityRef<User> _entityRefUser = default(EntityRef<User>);
    //    [Association(ThisKey = "UserId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefUser")]

    //    public User User
    //    {
    //        get { return _entityRefUser.Entity; }
    //        set { _entityRefUser.Entity = value; }
    //    }        
        
    //    [Column(IsDiscriminator = true, DbType = "varchar(50) not null")]
    //    protected string RoleType { get; set; }

    //    [Column(DbType = "varchar(255)")]
    //    public string RoleDescription { get; set; }

    
    [Table(Name = "Roles")]
    //[InheritanceMapping(Code = "UserRole", Type = typeof(EndConsumerRole), IsDefault = true)]
    //[InheritanceMapping(Code = "AdministratorRole", Type = typeof(AdministratorRole))]  
    public class Role
    {
        [Column(DbType = DatabaseType.IntIdentity, IsDbGenerated = true, IsPrimaryKey = true)]
        protected int Id { get; set; }

        [Column/*(IsDiscriminator = true, CanBeNull = false, DbType = "varchar(50)")*/]
        protected string RoleType { get; set; }

        [Column(DbType = "varchar(255)")]
        public string RoleName { get; set; }
    
        [Column(DbType = "varchar(255)")]
        public string RoleDescription { get; set; }
    }
    
}
