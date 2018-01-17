using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Affiliate.Ebay.Domain;
using Nop.Plugin.Affiliate.Ebay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay.Services
{
    public partial interface IAffiliateEbayService
    {
        IPagedList<CategoryEbayModel> GetAllCategories(int pageIndex = 0, int pageSize = int.MaxValue);
        void DeleteCategoryEbay(CategoryEbay categoryEbay);
        void InsertCategoryEbay(CategoryEbay categoryEbay);
        void UpdateCategoryEbay(CategoryEbay categoryEbay);
        CategoryEbay GetByEbayId(int ebayId);
        Product GetProductBySourceId(string productSourceId, int source);
        IList<CategoryEbay> GetAll();
        CategoryEbay Get(int id);
        CategoryMapping MapCategory(int id, int categoryId);
        void RemoveMapCategory(int id);
        IPagedList<Product> GetAllProduct(int pageIndex = 0, int pageSize = int.MaxValue, IList<int> categoryIds = null);
        SpecificationAttribute GetSpecificationAttributeByName(string name);
    }
}
