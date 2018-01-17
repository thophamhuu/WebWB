using System;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Plugins;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Data;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Infrastructure.Cache;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Services.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Core.Data;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip
{
    /// <summary>
    /// Fixed or by country & state & zip rate tax provider
    /// </summary>
    public class WB_FixedOrByCountryStateZipTaxProvider : BasePlugin, ITaxProvider
    {
        private readonly IWB_CountryStateZipService _taxRateService;
        private readonly IStoreContext _storeContext;
        private readonly WB_CountryStateZipObjectContext _objectContext;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly WB_FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;

        private readonly ICategoryService _categoryService;
        private readonly IWB_TaxCategoryMappingService _taxCategoryMappingService;
        public WB_FixedOrByCountryStateZipTaxProvider(IWB_CountryStateZipService taxRateService,
            IStoreContext storeContext,
            WB_CountryStateZipObjectContext objectContext,
            ICacheManager cacheManager,
            ISettingService settingService,
            WB_FixedOrByCountryStateZipTaxSettings countryStateZipSettings,
            ICategoryService categoryService,
            IWB_TaxCategoryMappingService taxCategoryMappingService
            )
        {
            this._taxRateService = taxRateService;
            this._storeContext = storeContext;
            this._objectContext = objectContext;
            this._cacheManager = cacheManager;
            this._settingService = settingService;
            this._countryStateZipSettings = countryStateZipSettings;
            this._categoryService = categoryService;
            this._taxCategoryMappingService = taxCategoryMappingService;
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            var _productMappingRepo = EngineContext.Current.Resolve<IRepository<ProductMapping>>();
            var productMapping = _productMappingRepo.TableNoTracking.FirstOrDefault(x => x.ProductId == calculateTaxRequest.Product.Id);
            var result = new CalculateTaxResult();
            var _taxCategoryMappingService = EngineContext.Current.Resolve<IWB_TaxCategoryMappingService>();
            if (productMapping != null)
            {
                //choose the tax rate calculation method
                if (!_countryStateZipSettings.CountryStateZipEnabled)
                {
                    if (calculateTaxRequest.Product.TaxCategoryId != 0)
                    {
                        result = new CalculateTaxResult
                        {
                            TaxRate = _settingService.GetSettingByKey<decimal>(string.Format("Tax.Worldbuy.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}", calculateTaxRequest.TaxCategoryId)),
                        };
                    }

                    else
                    {
                        var product = calculateTaxRequest.Product;
                        if (product.ProductCategories.Count == 0)
                        {

                        }
                        else
                        {
                            int taxCategoryId = _taxCategoryMappingService.GetTaxCategoryId(product);
                            var curRate = _settingService.GetSettingByKey<decimal>(string.Format("Tax.Worldbuy.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}", taxCategoryId));
                            result = new CalculateTaxResult
                            {
                                TaxRate = curRate,
                            };
                        }

                    }
                }
                else
                {
                    //the tax rate calculation by country & state & zip 

                    if (calculateTaxRequest.Address == null)
                    {
                        result.Errors.Add("Address is not set");
                        return result;
                    }

                    //first, load all tax rate records (cached) - loaded only once
                    const string cacheKey = WB_ModelCacheEventConsumer.ALL_TAX_RATES_MODEL_KEY;
                    var allTaxRates = _cacheManager.Get(cacheKey, () =>
                        _taxRateService
                            .GetAllTaxRates()
                            .Select(x => new WB_TaxRateForCaching
                            {
                                Id = x.Id,
                                StoreId = x.StoreId,
                                TaxCategoryId = x.TaxCategoryId,
                                CountryId = x.CountryId,
                                StateProvinceId = x.StateProvinceId,
                                Zip = x.Zip,
                                Percentage = x.Percentage,
                            }).ToList()
                        );

                    var storeId = _storeContext.CurrentStore.Id;
                    var taxCategoryId = calculateTaxRequest.TaxCategoryId;
                    var countryId = calculateTaxRequest.Address.Country != null ? calculateTaxRequest.Address.Country.Id : 0;
                    var stateProvinceId = calculateTaxRequest.Address.StateProvince != null
                        ? calculateTaxRequest.Address.StateProvince.Id
                        : 0;
                    var zip = calculateTaxRequest.Address.ZipPostalCode;

                    if (zip == null)
                        zip = string.Empty;
                    zip = zip.Trim();

                    var existingRates = allTaxRates.Where(taxRate => taxRate.CountryId == countryId && taxRate.TaxCategoryId == taxCategoryId).ToList();

                    //filter by store
                    //first, find by a store ID
                    var matchedByStore = existingRates.Where(taxRate => storeId == taxRate.StoreId).ToList();

                    //not found? use the default ones (ID == 0)
                    if (!matchedByStore.Any())
                        matchedByStore.AddRange(existingRates.Where(taxRate => taxRate.StoreId == 0));

                    //filter by state/province
                    //first, find by a state ID
                    var matchedByStateProvince = matchedByStore.Where(taxRate => stateProvinceId == taxRate.StateProvinceId).ToList();

                    //not found? use the default ones (ID == 0)
                    if (!matchedByStateProvince.Any())
                        matchedByStateProvince.AddRange(matchedByStore.Where(taxRate => taxRate.StateProvinceId == 0));

                    //filter by zip
                    var matchedByZip = matchedByStateProvince.Where(taxRate => (string.IsNullOrEmpty(zip) && string.IsNullOrEmpty(taxRate.Zip)) || zip.Equals(taxRate.Zip, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    if (!matchedByZip.Any())
                        matchedByZip.AddRange(matchedByStateProvince.Where(taxRate => string.IsNullOrWhiteSpace(taxRate.Zip)));

                    if (matchedByZip.Any())
                        result.TaxRate = matchedByZip[0].Percentage;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WB_FixedOrByCountryStateZip";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //database objects
            _objectContext.Install();

            //settings
            var settings = new WB_FixedOrByCountryStateZipTaxSettings
            {
                CountryStateZipEnabled = false
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fixed", "Fixed rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.TaxByCountryStateZip", "By Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.TaxCategoryMapping", "By Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategoryName", "Tax category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Rate", "Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Store", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Country.Hint", "The country.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.StateProvince", "State / province");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.StateProvince.Hint", "If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Zip", "Zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategory", "Tax category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategory.Hint", "The tax category.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Value", "Value");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Value.Hint", "The tax rate.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.IsPercent", "Is Calculated in percent ? ");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.IsPercent.Hint", "The Is Percent.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.AddRecord", "Add tax rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.AddRecordTitle", "New tax rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Percentage", "Percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Category", "Category");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //database objects
            _objectContext.Uninstall();

            //settings
            _settingService.DeleteSetting<WB_FixedOrByCountryStateZipTaxSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fixed");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.TaxByCountryStateZip");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.TaxCategoryMapping");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategoryName");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Rate");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Store");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Store.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Country.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.StateProvince");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.StateProvince.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Zip");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Zip.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategory");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategory.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Value");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Value.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.IsPercent");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.IsPercent.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.AddRecord");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.AddRecordTitle");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Percentage");
            this.DeletePluginLocaleResource("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Category");

            base.Uninstall();
        }
    }
}