using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [DataContract(Namespace = Base.DataContractNamespace)]   
    public class ItemInfoMessageEntity
    {
        [DataMember]
        public IList<ProductInfo> Products { get; set; }
        [DataMember]
        public IList<IngredientInfo> Ingredients { get; set; }
        [DataMember]
        public IList<BrandInfo> Brands { get; set; }
        [DataMember]
        public IList<CompanyInfo> Companies { get; set; }
        [DataMember]
        public IList<CountryInfo> Countries { get; set; }
        [DataMember]
        public IList<ConceptInfo> Concepts { get; set; }

    }

    [DataContract(Namespace = Base.DataContractNamespace)]
    public class ProductInfo
    {
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public string GTIN { get; set; }
        [DataMember]
        public string BrandName { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public int NumberAdvices { get; set; }
    }

    [DataContract(Namespace = Base.DataContractNamespace)]
    public class IngredientInfo
    {
        [DataMember]
        public int IngredientId { get; set; }
        [DataMember]
        public string IngredientName { get; set; }
        [DataMember]
        public int NumberAdvices { get; set; }
    }

    [DataContract(Namespace = Base.DataContractNamespace)]
    public class BrandInfo
    {
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public string BrandName { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public int NumberAdvices { get; set; }
    }

    [DataContract(Namespace = Base.DataContractNamespace)]
    public class CompanyInfo
    {
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public int NumberAdvices { get; set; }

    }

    [DataContract(Namespace = Base.DataContractNamespace)]
    public class CountryInfo
    {
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public string CountryName { get; set; }
        [DataMember]
        public int NumberAdvices { get; set; }
    }

    [DataContract(Namespace = Base.DataContractNamespace)]
    public class ConceptInfo
    {
        [DataMember]
        public int ConceptId { get; set; }
        [DataMember]
        public string ConceptName { get; set; }
        [DataMember]
        public int NumberAdvices { get; set; }
    }
}
