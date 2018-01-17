using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Core.Caching;

namespace Nop.Plugin.Affiliate.CategoryMap.Services
{
    public class CategoryMappingService : ICategoryMappingService
    {
        #region Fields
        private readonly IRepository<CategoryMapping> _categoryMappingRepo;
        private readonly ICategoryService _categoryService;
        private readonly ICacheManager _cacheManager;
        private const string CATEGORYMAPPING_PATTERN = "nop.plugins.categorymapping";
        private const string CATEGORYMAPPING_CATEGORY_PATTERN = "nop.plugins.categorymapping.source_{0}.category_{1}";
        #endregion

        #region Ctor
        public CategoryMappingService(IRepository<CategoryMapping> categoryMappingRepo, ICategoryService categoryService, ICacheManager cacheManager)
        {
            this._categoryMappingRepo = categoryMappingRepo;
            this._categoryService = categoryService;
            this._cacheManager = cacheManager;
        }

        public IEnumerable<CategoryMapping> GetAllByCategoryId(int sourceId, int categoryId)
        {
            var cacheKey = string.Format(CATEGORYMAPPING_CATEGORY_PATTERN, sourceId, categoryId);
            return _cacheManager.Get<IEnumerable<CategoryMapping>>(cacheKey, () =>
            {
                var query = _categoryMappingRepo.Table;
                if (sourceId > 0)
                    query = query.Where(x => x.SourceId == sourceId);
                if (categoryId > 0)
                    query = query.Where(x => x.CategoryId == categoryId);
                return query.ToList();
            });
        }

        public CategoryMapping GetById(int id)
        {
            return _categoryMappingRepo.GetById(id);
        }
        #endregion

        #region Methods
        #endregion

        #region Utilities
        #endregion

    }
}
