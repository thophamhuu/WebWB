using Nop.Core;
using Nop.Plugin.Affiliate.Amazon.Domain;
using Nop.Plugin.Affiliate.Amazon.Models;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    public interface ICategoryAmazonService
    {
        void Insert(CategoryAmazon record);
        void Insert(IList<CategoryAmazon> record);
        void Update(CategoryAmazon record);
        IPagedList<CategoryAmazonModel> GetAllCategories(CategorySearch model, int pageIndex = 0, int pageSize = int.MaxValue);
        IList<CategoryAmazonModel> GetAllCategoriesByParentBrowseNodeId(string parent);
        CategoryAmazon Get(int id);
        CategoryAmazon GetByBrowseNodeID(string browseNodeID);
        void Delete(CategoryAmazon record);
        CategoryMapping MapCategory(int id, int categoryId);
        void RemoveMapCategory(int id);
        void RemoveMapCategory(int id, int categoryId);
        IList<int> GetParentIdsByCategoryId(int categoryId, List<int> ids);
    }
}
