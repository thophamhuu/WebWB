using Nop.Core.Plugins;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using Nop.Services.Shipping.Tracking;
using System.Web.Routing;
using Nop.Services.Configuration;
using Nop.Plugin.Shipping.RateByDistrict.Services;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Plugin.Shipping.RateByDistrict.Data;
using Nop.Core.Domain.Shipping;
using Nop.Services.Localization;
using System.Linq;

namespace Nop.Plugin.Shipping.RateByDistrict
{
    public class RateByDistrictComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IShippingByDistrictService _shippingByDistrictService;
        private readonly RateByDistrictSettings _rateByDistrictSettings;
        private readonly IStoreContext _storeContext;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ShippingByDistrictObjectContext _objectContext;

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        private readonly IShippingByCategoryService _shippingByCategoryService;
        public RateByDistrictComputationMethod(
            ISettingService settingService,
            IShippingService shippingService,
            IShippingByDistrictService shippingByDistrictService,
            IShippingByCategoryService shippingByCategoryService,
            RateByDistrictSettings rateByDistrictSettings,
            IStoreContext storeContext,
            IPriceCalculationService priceCalculationService,
            ShippingByDistrictObjectContext objectContext,
            IProductService productService,
            ICategoryService categoryService
            )
        {
            this._settingService = settingService;
            this._shippingService = shippingService;
            this._shippingByDistrictService = shippingByDistrictService;
            this._shippingByCategoryService = shippingByCategoryService;
            this._rateByDistrictSettings = rateByDistrictSettings;
            this._storeContext = storeContext;
            this._priceCalculationService = priceCalculationService;
            this._objectContext = objectContext;

            this._categoryService = categoryService;
            this._productService = productService;
        }
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Offline;
            }
        }

        public IShipmentTracker ShipmentTracker
        {
            get
            {
                //uncomment a line below to return a general shipment tracker (finds an appropriate tracker by tracking number)
                //return new GeneralShipmentTracker(EngineContext.Current.Resolve<ITypeFinder>());
                return null;
            }
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "RateByDistrict";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.RateByDistrict.Controllers" }, { "area", null } };
        }
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            decimal? rateValue = 0;
            //if the "shipping calculation by weight" method is selected, the fixed rate isn't calculated
            if (!_rateByDistrictSettings.ShippingByDistrictEnabled)
                return null;

            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var restrictByCountryId = getShippingOptionRequest.ShippingAddress != null && getShippingOptionRequest.ShippingAddress.Country != null ? (int?)getShippingOptionRequest.ShippingAddress.Country.Id : null;

            var shippingMethods = _shippingService.GetAllShippingMethods(restrictByCountryId);

            var rates = new List<decimal>();
            foreach (var shippingMethod in shippingMethods)
            {
                var rate = GetRate(shippingMethod.Id);
                if (!rates.Contains(rate))
                    rates.Add(rate);
            }

            if (rates.Count == 1)
                rateValue += rates[0];

            foreach (var item in getShippingOptionRequest.Items)
            {
                var productCategories = _categoryService.GetProductCategoriesByProductId(item.ShoppingCartItem.ProductId);
                if (productCategories != null)
                {
                    var productCategory = productCategories.LastOrDefault();

                    decimal rateByCategory = GetRateByCategory(productCategory.CategoryId) * item.ShoppingCartItem.Quantity;
                    if (rateByCategory > 0)
                        rateValue += rateByCategory;
                }
            }
            return rateValue;
        }

        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || !getShippingOptionRequest.Items.Any())
            {
                response.AddError("No shipment items");
                return response;
            }

            //choose the shipping rate calculation method
            if (_rateByDistrictSettings.ShippingByDistrictEnabled)
            {
                //shipping rate calculation by products weight

                if (getShippingOptionRequest.ShippingAddress == null)
                {
                    response.AddError("Shipping address is not set");
                    return response;
                }

                var storeId = getShippingOptionRequest.StoreId;

                if (storeId == 0)
                    storeId = _storeContext.CurrentStore.Id;

                var countryId = getShippingOptionRequest.ShippingAddress.CountryId.HasValue ? getShippingOptionRequest.ShippingAddress.CountryId.Value : 0;
                var stateProvinceId = getShippingOptionRequest.ShippingAddress.StateProvinceId.HasValue ? getShippingOptionRequest.ShippingAddress.StateProvinceId.Value : 0;
                var warehouseId = getShippingOptionRequest.WarehouseFrom != null ? getShippingOptionRequest.WarehouseFrom.Id : 0;
                var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                var subTotal = decimal.Zero;

                foreach (var packageItem in getShippingOptionRequest.Items)
                {
                    if (packageItem.ShoppingCartItem.IsFreeShipping)
                        continue;
                    //TODO we should use getShippingOptionRequest.Items.GetQuantity() method to get subtotal
                    subTotal += _priceCalculationService.GetSubTotal(packageItem.ShoppingCartItem);
                }

                var weight = _shippingService.GetTotalWeight(getShippingOptionRequest);

                var shippingMethods = _shippingService.GetAllShippingMethods(countryId);
                foreach (var shippingMethod in shippingMethods)
                {
                    var rate = GetRate(subTotal, shippingMethod.Id, storeId, warehouseId, countryId, stateProvinceId, zip);

                    if (!rate.HasValue) continue;

                    var shippingOption = new ShippingOption
                    {
                        Name = shippingMethod.GetLocalized(x => x.Name),
                        Description = shippingMethod.GetLocalized(x => x.Description),
                        Rate = rate.Value
                    };

                    foreach (var item in getShippingOptionRequest.Items)
                    {
                        var productCategories = _categoryService.GetProductCategoriesByProductId(item.ShoppingCartItem.ProductId);
                        if (productCategories != null)
                        {
                            var productCategory = productCategories.LastOrDefault();

                            decimal rateByCategory = GetRateByCategory(productCategory.CategoryId) * item.ShoppingCartItem.Quantity;
                            if (rateByCategory > 0)
                                shippingOption.Rate += rateByCategory;
                        }
                    }

                    response.ShippingOptions.Add(shippingOption);
                }
            }
            else
            {
                //shipping rate calculation by fixed rate

                var restrictByCountryId = getShippingOptionRequest.ShippingAddress != null && getShippingOptionRequest.ShippingAddress.Country != null ? (int?)getShippingOptionRequest.ShippingAddress.Country.Id : null;
                var shippingMethods = _shippingService.GetAllShippingMethods(restrictByCountryId);

                foreach (var shippingMethod in shippingMethods)
                {
                    var shippingOption = new ShippingOption
                    {
                        Name = shippingMethod.GetLocalized(x => x.Name),
                        Description = shippingMethod.GetLocalized(x => x.Description),
                        Rate = GetRate(shippingMethod.Id)
                    };
                    response.ShippingOptions.Add(shippingOption);
                }
            }
            return response;
        }

        #region Utilities
        private decimal GetRateByCategory(int categoryId)
        {
            decimal rate = 0;
            var category = _categoryService.GetCategoryById(categoryId);
            while (category != null)
            {
                var shippingCategory = _shippingByCategoryService.GetCategoryByCategoryId(category.Id);
                if (shippingCategory != null)
                {
                    if (shippingCategory.AdditionalFixedCost > 0)
                    {
                        rate = shippingCategory.AdditionalFixedCost;
                        break;
                    }
                    else
                    {
                        var shippingType = _shippingByCategoryService.GetProductTypeByCategoryId(shippingCategory.CategoryId);
                        if (shippingType != null)
                        {
                            if (shippingType.AdditionalFixedCost > 0)
                            {
                                rate = shippingType.AdditionalFixedCost;
                                break;
                            }
                        }
                    }
                }
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }

            return rate;
        }
        /// <summary>
        /// Get fixed rate
        /// </summary>
        /// <param name="shippingMethodId">Shipping method ID</param>
        /// <returns>Rate</returns>
        private decimal GetRate(int shippingMethodId)
        {
            var key = string.Format("ShippingRateComputationMethod.RateByDistrict.ShippingMethodId{0}", shippingMethodId);
            var rate = _settingService.GetSettingByKey<decimal>(key);
            return rate;
        }

        /// <summary>
        /// Get rate by weight
        /// </summary>
        /// <param name="subTotal">Subtotal</param>
        /// <param name="weight">Weight</param>
        /// <param name="shippingMethodId">Shipping method ID</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="warehouseId">Warehouse ID</param>
        /// <param name="countryId">Country ID</param>
        /// <param name="stateProvinceId">State/Province ID</param>
        /// <param name="zip">Zip code</param>
        /// <returns>Rate</returns>
        private decimal? GetRate(decimal subTotal, int shippingMethodId,
            int storeId, int warehouseId, int countryId, int stateProvinceId, string zip)
        {
            var shippingByDistrictRecord = _shippingByDistrictService.FindRecord(shippingMethodId,
                storeId, warehouseId, countryId, stateProvinceId, zip);
            if (shippingByDistrictRecord == null)
            {
                return decimal.Zero;
            }

            //additional fixed cost
            var shippingTotal = shippingByDistrictRecord.AdditionalFixedCost;
            //charge amount per weight unit
            if (shippingTotal < decimal.Zero)
                shippingTotal = decimal.Zero;
            return shippingTotal;
        }

        public override void Install()
        {
            //settings
            var settings = new RateByDistrictSettings
            {
                ShippingByDistrictEnabled = false
            };
            _settingService.SaveSetting(settings);

            //database objects
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.ShippingByDistrict", "By Weight");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fixed", "Fixed Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Rate", "Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Store", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Warehouse", "Warehouse");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Warehouse.Hint", "If an asterisk is selected, then this shipping rate will apply to all warehouses.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Country.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.StateProvince", "State / province");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.StateProvince.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Zip", "Zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.ShippingMethod", "Shipping method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.ShippingMethod.Hint", "Choose shipping method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.From", "Order weight from");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.From.Hint", "Order weight from.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.To", "Order weight to");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.To.Hint", "Order weight to.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.AdditionalFixedCost", "Additional fixed cost");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.AdditionalFixedCost.Hint", "Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don't want an additional fixed cost to be applied.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.LowerWeightLimit", "Lower weight limit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.LowerWeightLimit.Hint", "Lower weight limit. This field can be used for \"per extra weight unit\" scenarios.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.PercentageRateOfSubtotal", "Charge percentage (of subtotal)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.PercentageRateOfSubtotal.Hint", "Charge percentage (of subtotal).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.RatePerWeightUnit", "Rate per weight unit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.RatePerWeightUnit.Hint", "Rate per weight unit.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.LimitMethodsToCreated", "Limit shipping methods to configured ones");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.LimitMethodsToCreated.Hint", "If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they'll be able to choose any existing shipping options even they've not configured here (zero shipping fee in this case).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.DataHtml", "Data");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.AddRecord", "Add record");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Formula", "Formula to calculate rates");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.RateByDistrict.Formula.Value", "[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<RateByDistrictSettings>();

            //database objects
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Rate");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Store");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Store.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Warehouse");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Warehouse.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Country.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.StateProvince");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.StateProvince.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Zip");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.Zip.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.ShippingMethod");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.ShippingMethod.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.From");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.From.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.To");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.To.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.AdditionalFixedCost");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.AdditionalFixedCost.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.LowerWeightLimit");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.LowerWeightLimit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.PercentageRateOfSubtotal");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.PercentageRateOfSubtotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.RatePerWeightUnit");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.RatePerWeightUnit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.LimitMethodsToCreated");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.LimitMethodsToCreated.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Fields.DataHtml");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.AddRecord");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Formula");
            this.DeletePluginLocaleResource("Plugins.Shipping.RateByDistrict.Formula.Value");

            base.Uninstall();
        }
        #endregion
    }
}
