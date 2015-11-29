using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Tags")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    [InheritanceMapping(Code = "AdviceTag", Type = typeof(AdviceTag), IsDefault = true)]
    [KnownType(typeof(AdviceTag))]
    public abstract class TagBase
    {
        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true, AutoSync = AutoSync.OnInsert)]
        [DataMember(Name = "id")]
        public int? Id { get; set; }

        [Column(IsDiscriminator = true, DbType = "varchar(55) not null")]
        protected virtual string TagType { get; set; }

        [Column(DbType = "NVARCHAR(50)")]
        public string Name { get; set; }
    }
}
