using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class WB_ModelCacheEventConsumer : 
        //tax rates
        IConsumer<EntityInserted<WB_TaxRate>>,
        IConsumer<EntityUpdated<WB_TaxRate>>,
        IConsumer<EntityDeleted<WB_TaxRate>>,
        //tax category mappings
        IConsumer<EntityInserted<WB_TaxCategoryMapping>>,
        IConsumer<EntityUpdated<WB_TaxCategoryMapping>>,
        IConsumer<EntityDeleted<WB_TaxCategoryMapping>>,
        //tax category
        IConsumer<EntityDeleted<TaxCategory>>
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        public const string ALL_TAX_RATES_MODEL_KEY = "Nop.plugins.tax.worldbuy.fixedorbycountrystateziptaxrate.all";
        public const string TAXRATE_ALL_KEY = "Nop.plugins.tax.worldbuy.fixedorbycountrystateziptaxrate.all-{0}-{1}";
        public const string TAXRATE_PATTERN_KEY = "Nop.plugins.tax.worldbuy.fixedorbycountrystateziptaxrate.";
        public const string TAXCATEGORYMAPPING_PATTERN_KEY = "Nop.plugins.tax.worldbuy.taxcategorymapping.";

        private readonly ICacheManager _cacheManager;
        private readonly IWB_CountryStateZipService _taxRateService;
        private readonly IWB_TaxCategoryMappingService _taxCategoryMappingService;
        private readonly ISettingService _settingService;

        public WB_ModelCacheEventConsumer(IWB_CountryStateZipService taxRateService, IWB_TaxCategoryMappingService taxCategoryMappingService, ISettingService settingService)
        {
            //TODO inject static cache manager using constructor
            this._cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
            
            this._taxRateService = taxRateService;
            this._taxCategoryMappingService = taxCategoryMappingService;

            this._settingService = settingService;
        }

        //tax rates
        public void HandleEvent(EntityInserted<WB_TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<WB_TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<WB_TaxRate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
        }

        //tax category
        public void HandleEvent(EntityDeleted<TaxCategory> eventMessage)
        {
            if (eventMessage.Entity == null)
                return;

            //delete an appropriate record when tax category is deleted
            var recordsToDelete = _taxRateService.GetAllTaxRates().Where(tr => tr.TaxCategoryId == eventMessage.Entity.Id).ToList();
            foreach (var taxRate in recordsToDelete)
            {
                _taxRateService.DeleteTaxRate(taxRate);
            }

            var settingKey = string.Format("Tax.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}", eventMessage.Entity.Id);
            var setting = _settingService.GetSetting(settingKey);
            if (setting != null)
                _settingService.DeleteSetting(setting);
        }

        public void HandleEvent(EntityInserted<WB_TaxCategoryMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXCATEGORYMAPPING_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdated<WB_TaxCategoryMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXCATEGORYMAPPING_PATTERN_KEY);
        }

        public void HandleEvent(EntityDeleted<WB_TaxCategoryMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(TAXCATEGORYMAPPING_PATTERN_KEY);
        }
    }
}
