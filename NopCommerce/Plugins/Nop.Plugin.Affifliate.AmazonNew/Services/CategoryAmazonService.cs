using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Affiliate.Amazon.Domain;
using Nop.Core.Data;
using Nop.Plugin.Affiliate.Amazon.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    public partial class CategoryAmazonService : ICategoryAmazonService
    {
        #region Fields
        private const string _cacheKey = "CACHE_CATEGORIES_AMAZON";
        private readonly IRepository<CategoryAmazon> _categoryAmazonRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategoryMapping> _categoryMapRepository;
        #endregion
        #region ctr
        public CategoryAmazonService(IRepository<CategoryAmazon> categoryAmazonRepository, IRepository<Category> categoryRepository, IRepository<CategoryMapping> categoryMapRepository)
        {
            this._categoryAmazonRepository = categoryAmazonRepository;
            this._categoryRepository = categoryRepository;
            this._categoryMapRepository = categoryMapRepository;
        }
        #endregion
        public void Delete(CategoryAmazon record)
        {
            throw new NotImplementedException();
        }

        public CategoryAmazon Get(int id)
        {
            return _categoryAmazonRepository.Table.FirstOrDefault(m => m.Id == id);
        }
        public CategoryAmazon GetByBrowseNodeID(string browseNodeID)
        {
            return _categoryAmazonRepository.Table.FirstOrDefault(m => m.BrowseNodeID.Equals(browseNodeID));
        }
        public void Insert(CategoryAmazon record)
        {
            _categoryAmazonRepository.Insert(record);
        }
        public void Insert(IList<CategoryAmazon> record)
        {
            _categoryAmazonRepository.Insert(record);
        }

        public IPagedList<CategoryAmazonModel> GetAllCategories(CategorySearch model, int pageIndex = 1, int pageSize = 100)
        {
            var _amazonProvider = EngineContext.Current.Resolve<IAmazonProvider>();
            var result = _amazonProvider.GetAllCategories(model);
            int count = 0;
            
            count = result.Count();
            var data = result.OrderBy(x => x.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            if (data != null)
            {
                var ids = data.Select(x => x.Id).ToArray();
                var maps = _categoryMapRepository.TableNoTracking.Where(m => m.SourceId == 2 && ids.Contains(m.CategorySourceId)).ToList();
                data.ForEach(x =>
                {
                    var map = maps.FirstOrDefault(m => m.CategorySourceId == x.Id);
                    if (map != null)
                    {
                        var cate = _categoryRepository.GetById(map.CategoryId);
                        if (cate != null)
                        {
                            x.CategoryID = cate.Id;
                            x.CategoryMapID = map.Id;
                            x.CategoryName = cate.Name;
                        }
                    }
                });
            }
            var pagedList = new PagedList<CategoryAmazonModel>(data, pageIndex, pageSize, count);
            return pagedList;
        }
        public IList<CategoryAmazonModel> GetAllCategoriesByParentBrowseNodeId(string parent)
        {
            var childs = new List<CategoryAmazonModel>();

            childs = _categoryAmazonRepository.TableNoTracking.Where(x => x.ParentBrowseNodeID == parent).Select(x => new CategoryAmazonModel
            {
                BrowseNodeID = x.BrowseNodeID,
                Name = x.Name,
                Id = x.Id,
                IsCategoryRoot = x.IsCategoryRoot,
                ParentBrowseNodeID = x.ParentBrowseNodeID,
                SearchIndex = x.SearchIndex
            }).ToList();

            return childs;
        }
        public void Update(CategoryAmazon record)
        {
            _categoryAmazonRepository.Update(record);
            var _categoryAmazonProvider = EngineContext.Current.Resolve<IAmazonProvider>();
            _categoryAmazonProvider.ClearCacheCategory();

        }
        public CategoryMapping MapCategory(int id, int categoryId)
        {
            var categoryMap = _categoryMapRepository.Table.FirstOrDefault(x => x.CategoryId == categoryId && x.CategorySourceId == id) ?? new CategoryMapping
            {
                CategoryId = categoryId,
                CategorySourceId = id,
                SourceId = 2
            };
            if (categoryMap.Id == 0)
            {
                _categoryMapRepository.Insert(categoryMap);
            }
            else
            {
                categoryMap.CategoryId = categoryId;
                _categoryMapRepository.Update(categoryMap);
            }
            return categoryMap;
        }
        public void RemoveMapCategory(int id)
        {
            var categoryMap = _categoryMapRepository.GetById(id);
            if (categoryMap != null)
            {
                _categoryMapRepository.Delete(categoryMap);
            }
            else
            {
                throw new ArgumentNullException("categoryMap");
            }
        }

        public void RemoveMapCategory(int sourceId, int categoryId)
        {
            var categoryMap = _categoryMapRepository.Table.FirstOrDefault(x => x.CategoryId == categoryId && x.CategorySourceId == sourceId);
            if (categoryMap != null)
            {
                _categoryMapRepository.Delete(categoryMap);
                var _categoryAmazonProvider = EngineContext.Current.Resolve<IAmazonProvider>();
                _categoryAmazonProvider.ClearCacheCategory();
            }
            else
            {
                throw new ArgumentNullException("categoryMap");
            }
        }


        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        public virtual IList<int> GetParentIdsByCategoryId(int categoryId, List<int> ids)
        {

            var cate = _categoryRepository.GetById(categoryId);
            if (cate != null && cate.ParentCategoryId != 0)
            {
                ids.Add(cate.ParentCategoryId);
                GetParentIdsByCategoryId(cate.ParentCategoryId, ids);
            }
            return ids;
        }

        private IEnumerable<CategoryAmazonModel> TreeView(IList<CategoryAmazonModel> source, string parentId = "", string parentName = "")
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var result = new List<CategoryAmazonModel>();
            if (parentName != null && parentName != "")
                parentName = parentName + " >> ";
            foreach (var cat in source.Where(c => c.ParentBrowseNodeID == parentId || parentId == "").ToList())
            {
                cat.Name = parentName + cat.Name;
                if (!result.Contains(cat))
                {
                    result.Add(cat);
                    result.AddRange(TreeView(source, cat.BrowseNodeID, cat.Name));
                }
            }

            return result;
        }
    }
}
