using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Infrastructure.Cache;
using Nop.Services.Events;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services
{
    /// <summary>
    /// Tax rate service
    /// </summary>
    public partial class WB_TaxCategoryMappingService : IWB_TaxCategoryMappingService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WB_TaxCategoryMapping> _taxCategoryMappingRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="taxCategoryMappingRepository">Tax Category Mapping repository</param>
        public WB_TaxCategoryMappingService(IEventPublisher eventPublisher,
            ICacheManager cacheManager,
            IRepository<WB_TaxCategoryMapping> taxCategoryMappingRepository)
        {
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
            this._taxCategoryMappingRepository = taxCategoryMappingRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a Tax Category Mapping
        /// </summary>
        /// <param name="taxCategoryMapping">Tax Category Mapping</param>
        public virtual void DeleteTaxCategoryMapping(WB_TaxCategoryMapping taxCategoryMapping)
        {
            if (taxCategoryMapping == null)
                throw new ArgumentNullException("taxCategoryMapping");

            _taxCategoryMappingRepository.Delete(taxCategoryMapping);

            //event notification
            _eventPublisher.EntityDeleted(taxCategoryMapping);
        }

        /// <summary>
        /// Gets all Tax Category Mappings
        /// </summary>
        /// <returns>Tax Category Mappings</returns>
        public virtual IPagedList<WB_TaxCategoryMapping> GetAllTaxCategoryMappings(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(WB_ModelCacheEventConsumer.TAXCATEGORYMAPPING_PATTERN_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from tr in _taxCategoryMappingRepository.Table
                            orderby tr.StoreId, tr.TaxCategoryId, tr.CategoryId
                            select tr;
                var records = new PagedList<WB_TaxCategoryMapping>(query, pageIndex, pageSize);
                return records;
            });
        }

        /// <summary>
        /// Gets a Tax Category Mapping
        /// </summary>
        /// <param name="taxCategoryMappingId">Tax Category Mapping identifier</param>
        /// <returns>Tax Category Mapping</returns>
        public virtual WB_TaxCategoryMapping GetTaxCategoryMappingById(int taxCategoryMappingId)
        {
            if (taxCategoryMappingId == 0)
                return null;

            return _taxCategoryMappingRepository.GetById(taxCategoryMappingId);
        }

        /// <summary>
        /// Inserts a Tax Category Mapping
        /// </summary>
        /// <param name="taxCategoryMapping">Tax Category Mapping</param>
        public virtual void InsertTaxCategoryMapping(WB_TaxCategoryMapping taxCategoryMapping)
        {
            if (taxCategoryMapping == null)
                throw new ArgumentNullException("taxCategoryMapping");

            _taxCategoryMappingRepository.Insert(taxCategoryMapping);

            //event notification
            _eventPublisher.EntityInserted(taxCategoryMapping);
        }

        /// <summary>
        /// Updates the Tax Category Mapping
        /// </summary>
        /// <param name="taxCategoryMapping">Tax Category Mapping</param>
        public virtual void UpdateTaxCategoryMapping(WB_TaxCategoryMapping taxCategoryMapping)
        {
            if (taxCategoryMapping == null)
                throw new ArgumentNullException("taxCategoryMapping");

            _taxCategoryMappingRepository.Update(taxCategoryMapping);

            //event notification
            _eventPublisher.EntityUpdated(taxCategoryMapping);
        }
        public int GetTaxCategoryId(Product product)
        {
            var _categoryService = EngineContext.Current.Resolve<ICategoryService>();
            int taxCategoryId = 0;
            var cateIds = product.ProductCategories;
            var taxcategoryMappings = this.GetAllTaxCategoryMappings();
            if (cateIds != null)
            {
                foreach (var cateId in cateIds)
                {
                    var category = _categoryService.GetCategoryById(cateId.CategoryId);
                    while (category != null)
                    {
                        var taxcategoryMapping = taxcategoryMappings.FirstOrDefault(x => x.CategoryId == category.Id);
                        if (taxcategoryMapping != null)
                        {
                            taxCategoryId = taxcategoryMapping.TaxCategoryId;
                            break;
                        }
                        else
                        {
                            category = _categoryService.GetCategoryById(category.ParentCategoryId);
                        }
                    }
                }
            }
            return taxCategoryId;
        }
        #endregion
    }
}