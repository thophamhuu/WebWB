using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Affiliate.Amazon.Domain;
using Nop.Plugin.Affiliate.Amazon.Models;
using Nop.Plugin.Affiliate.Amazon.Models.Response;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Affiliate.CategoryMap.Services;
using Nop.Services.Catalog;
using System.Collections.Generic;
using System.Web;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    public partial interface IAmazonService
    {
        BrowseNodeLookupResponse BrowseNodeLookup( int accountIndex, string browseNodeId, string responseGroup = "BrowseNodeInfo");
        ItemSearchResponse ItemSearch(int accountIndex, string searchIndex, string browseNode, string Keywords = "", string responseGroup = "Small", params KeyValuePair<string, string>[] param);
        ItemLookupResponse ItemLookup(int accountIndex, string itemId, string responseGroup = "", params KeyValuePair<string, string>[] param);

        void SyncCategory(IRepository<CategoryAmazon> _categoryAmazonRepo, string browseNodeID = "");
        void SyncProducts(ICategoryService _categoryService,
            ICategoryMappingService _categoryMappingService,
            IRepository<CategoryAmazon> _categoryAmazonRepo,
            int storeId, int categoryId, string keywords = "", SyncProperties syncProperties = SyncProperties.All);
        void UpdateProducts(IRepository<ProductMapping> _productMappingRepo,
            int storeId, int categoryId, SyncProperties syncProperties);
        void SyncProduct(IRepository<Product> _productRepo,
            IRepository<ProductMapping> _productMappingRepo,
            int storeId, int id, SyncProperties properties);
    }
}
