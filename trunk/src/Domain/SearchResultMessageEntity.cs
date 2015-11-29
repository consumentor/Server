using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Domain
{
    [DataContract(Namespace = Base.DataContractNamespace)]
    public class SearchResultMessageEntity
    {
        //NOTE: Should be Enums
        public static readonly string FreeSearch = "freeSearch";
        public static readonly string GtinSearch = "gtinSearch";
        public static readonly string ProductSearch = "productSearch";
        public static readonly string BrandSearch = "brandSearch";
        public static readonly string CompanySearch = "companySearch";
        public static readonly string CountrySearch = "countrySearch";
        public static readonly string ConceptSearch = "conceptSearch";

        [DataMember(Name = "searchType")]
        public string SearchType { get; set; }

        /*[DataMember(Name = "searchType")]
        public SearchType SearchType { get; set; }*/

        [DataMember(Name = "products")]
        public IList<Product> Products { get; set; }

        [DataMember(Name = "ingredients")]
        public IList<Ingredient> Ingredients { get; set; }

        [DataMember(Name = "brands")]
        public IList<Brand> Brands { get; set; }

        [DataMember(Name = "companies")]
        public IList<Company> Companies { get; set; }

        [DataMember(Name = "countries")]
        public IList<Country> Countries { get; set; }

        [DataMember(Name = "concepts")]
        public IList<Concept> Concepts { get; set; }

        public bool HasResults()
        {
            return (Products != null && Products.Count > 0)
                || (Ingredients != null && Ingredients.Count > 0)
                || (Brands != null && Brands.Count > 0)
                || (Companies != null && Companies.Count > 0)
                || (Countries != null && Countries.Count > 0)
                || (Concepts != null && Concepts.Count > 0);
        }
    }
}
