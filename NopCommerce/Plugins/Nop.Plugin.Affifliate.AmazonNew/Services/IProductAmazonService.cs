using Nop.Core;
using Nop.Plugin.Affiliate.Amazon.Models;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    public interface IProductAmazonService
    {
        IPagedList<ProductAmazonModel> GetAllByCategoryId(ProductParameter model, int pageIndex = 1, int pageSize = 25);
        IEnumerable<ProductMapping> GetAllProductMappingByCategoryId(int categoryId, bool? isPublished = null, int page = 1, int size = 100);
    }
}
