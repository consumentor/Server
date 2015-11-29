using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [Table(Name = "Countries")]
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class Country : IAdviceable<CountryAdvice>
    {
        public Country()
        {
            SetupEntitySets();
        }

        private void SetupEntitySets()
        {
            _entitySetCountryAdvices = new EntitySet<CountryAdvice>(onAdd => onAdd.Country = this, onRemove => { onRemove.Country = null; });
            _entitySetCountryStatistics = new EntitySet<CountryStatistic>(onAdd => onAdd.Country = this, onRemove => { onRemove.Country = null; });
        }

        [OnDeserializing]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Parameter 'context' of 'DomainObject.OnDeserializing(StreamingContext)' is never used. Remove the parameter or use it in the method body.")]
        private void OnDeserializing(StreamingContext context)
        {
            SetupEntitySets();
        }

        [Column(DbType = "int IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        //[Column(DbType = "NVarChar(255)")]
        //[DataMember(Name = "countryName")]
        //public string Name { get; set; }

        [Column(CanBeNull = false)]
        public int? CountryCodeId { get; set; }
        private EntityRef<CountryCode> _entityRefCountryCode = default(EntityRef<CountryCode>);
        [Association(ThisKey = "CountryCodeId", OtherKey = "Id", IsForeignKey = true, Storage = "_entityRefCountryCode")]

        [DataMember(Name = "countryCode")]
        public CountryCode CountryCode
        {
            get { return _entityRefCountryCode.Entity; }
            set { _entityRefCountryCode.Entity = value; }
        }

        [Column(CanBeNull = true)]
        public string Capital { get; set; }

        [Column(CanBeNull = true)]
        [DataMember(Name = "latitude")]
        public string Latitude { get; set; }

        [Column(CanBeNull = true)]
        [DataMember(Name = "longitude")]
        public string Longitude { get; set; }

        private EntitySet<CountryAdvice> _entitySetCountryAdvices;
        [Association(ThisKey = "Id", OtherKey = "CountrysId", Storage = "_entitySetCountryAdvices")]
        [DataMember(Name = "countryAdvices")]
        public IList<CountryAdvice> CountryAdvices
        {
            get
            {
                //Just include the published advices.
                var resultEntitySetCountryAdvices = new List<CountryAdvice>();
                foreach (var countryAdvice in _entitySetCountryAdvices)
                {
                    if (countryAdvice.Published)
                        resultEntitySetCountryAdvices.Add(countryAdvice);
                }
                return resultEntitySetCountryAdvices;
            }
            set { _entitySetCountryAdvices.Assign(value); }
        }

        private EntitySet<CountryStatistic> _entitySetCountryStatistics;
        [Association(ThisKey = "Id", OtherKey = "CountryId", Storage = "_entitySetCountryStatistics")]
        public IList<CountryStatistic> CountryStatistics
        {
            get { return _entitySetCountryStatistics; }
            set { _entitySetCountryStatistics.Assign(value); }
        }

        [Column]
        [DataMember(Name = "lastUpdated")]
        public DateTime LastUpdated { get; set; }

        public void AddAdvice(CountryAdvice advice)
        {
            _entitySetCountryAdvices.Add(advice);
        }

        public void RemoveAdvice(CountryAdvice adviceToRemove)
        {
            _entitySetCountryAdvices.Remove(adviceToRemove);
        }
    }
}
