using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Searchterms")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Searchterm 
    {
        public Searchterm()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetSearchStatistics = new EntitySet<SearchStatistic>(onAdd => onAdd.Searchterm = this, onRemove => { onRemove.Searchterm = null; });
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false, DbType = DatabaseType.IntIdentity)]
        public int Id { get; set; }

        [Column(CanBeNull = false, DbType = DatabaseType.Nvarchar50)]
        [DataMember(Name = "searchstring")]
        public string Searchstring { get; set; }

        private EntitySet<SearchStatistic> _entitySetSearchStatistics;
        [Association(ThisKey = "Id", OtherKey = "SearchtermId", Storage = "_entitySetSearchStatistics")]
        public IList<SearchStatistic> SearchStatistics
        {
            get { return _entitySetSearchStatistics; }
            set { _entitySetSearchStatistics.Assign(value); }

        }
    }
}
