using System.Collections.Generic;
using Nop.Plugin.Affiliate.Amazon.Models;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    public interface IAmazonProvider
    {
        IEnumerable<CategoryAmazonModel> GetAllCategories(CategorySearch model);
        IList<int> GetAllProduct(ProductParameter model);
        IList<int> GetAllProductMapping(ProductParameter model);
        void ClearCacheCategory();
        void ClearCacheProduct();
        void ClearCacheProductMapping();
    }
}
