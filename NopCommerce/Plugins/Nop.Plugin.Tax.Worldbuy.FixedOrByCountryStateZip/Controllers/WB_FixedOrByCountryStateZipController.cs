using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Models;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using Nop.Services.Catalog;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Controllers
{
    [AdminAuthorize]
    public class WB_FixedOrByCountryStateZipController : BasePluginController
    {
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICategoryService _categoryService;
        private readonly IWB_CountryStateZipService _taxRateService;
        private readonly IWB_TaxCategoryMappingService _taxCategoryMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly WB_FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;

        public WB_FixedOrByCountryStateZipController(ITaxCategoryService taxCategoryService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ICategoryService categoryService,
            IWB_CountryStateZipService taxRateService,
            IWB_TaxCategoryMappingService taxCategoryMappingService,
            IPermissionService permissionService,
            IStoreService storeService,
            ISettingService settingService,
            WB_FixedOrByCountryStateZipTaxSettings countryStateZipSettings)
        {
            this._taxCategoryService = taxCategoryService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._categoryService = categoryService;
            this._taxRateService = taxRateService;
            this._taxCategoryMappingService = taxCategoryMappingService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._settingService = settingService;
            this._countryStateZipSettings = countryStateZipSettings;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //little hack here
            //always set culture to 'en-US' (Telerik has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
            CommonHelper.SetTelerikCulture();

            base.Initialize(requestContext);
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            if (!taxCategories.Any())
                return Content("No tax categories can be loaded");

            var model = new ConfigurationModel { CountryStateZipEnabled = _countryStateZipSettings.CountryStateZipEnabled };
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            var stores = _storeService.GetAllStores();
            foreach (var s in stores)
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            //tax categories
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString() });
            //countries
            var countries = _countryService.GetAllCountries(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });
            var defaultCountry = countries.FirstOrDefault();
            if (defaultCountry != null)
            {
                var states = _stateProvinceService.GetStateProvincesByCountryId(defaultCountry.Id);
                foreach (var s in states)
                    model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            }

            var categories = _categoryService.GetAllCategories("", 1);
            model.AvailableCategories = categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.GetFormattedBreadCrumb(_categoryService)
            }).ToList();

            return View("~/Plugins/Tax.Worldbuy.FixedOrByCountryStateZip/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public ActionResult SaveMode(bool value)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            //save settings
            _countryStateZipSettings.CountryStateZipEnabled = value;
            _settingService.SaveSetting(_countryStateZipSettings);

            return Json(new
            {
                Result = true
            }, JsonRequestBehavior.AllowGet);
        }

        #region Fixed tax

        [NonAction]
        protected decimal GetFixedTaxRateValue(int taxCategoryId)
        {
            var rate = _settingService.GetSettingByKey<decimal>(string.Format("Tax.Worldbuy.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}", taxCategoryId));
            return rate;
        }

        [NonAction]
        protected bool GetIsAbsoluteValue(int taxCategoryId)
        {
            var isAbsolute = _settingService.GetSettingByKey<bool>(string.Format("Tax.Worldbuy.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}.IsAbsolute", taxCategoryId));
            return isAbsolute;
        }
        [HttpPost]
        public ActionResult FixedRatesList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return ErrorForKendoGridJson("Access denied");

            var taxRateModels = _taxCategoryService.GetAllTaxCategories().Select(taxCategory => new WB_FixedTaxRateModel
            {
                TaxCategoryId = taxCategory.Id,
                TaxCategoryName = taxCategory.Name,
                Rate = GetFixedTaxRateValue(taxCategory.Id),
                IsAbsolute=GetIsAbsoluteValue(taxCategory.Id)
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = taxRateModels,
                Total = taxRateModels.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult FixedRateUpdate(WB_FixedTaxRateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxCategoryId = model.TaxCategoryId;
            var rate = model.Rate;
            var isAbsolute = model.IsAbsolute;
            _settingService.SetSetting(string.Format("Tax.Worldbuy.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}", taxCategoryId), rate);
            _settingService.SetSetting(string.Format("Tax.Worldbuy.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}.IsAbsolute", taxCategoryId), isAbsolute);
            return new NullJsonResult();
        }

        #endregion

        #region Tax by country/state/zip

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult RatesByCountryStateZipList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return ErrorForKendoGridJson("Access denied");

            var records = _taxRateService.GetAllTaxRates(command.Page - 1, command.PageSize);
            var taxRatesModel = records
                .Select(x =>
                {
                    var m = new WB_CountryStateZipModel
                    {
                        Id = x.Id,
                        StoreId = x.StoreId,
                        TaxCategoryId = x.TaxCategoryId,
                        CountryId = x.CountryId,
                        StateProvinceId = x.StateProvinceId,
                        Zip = x.Zip,
                        Percentage=x.Percentage
                    };
                    //store
                    var store = _storeService.GetStoreById(x.StoreId);
                    m.StoreName = store != null ? store.Name : "*";
                    //tax category
                    var tc = _taxCategoryService.GetTaxCategoryById(x.TaxCategoryId);
                    m.TaxCategoryName = tc != null ? tc.Name : "";
                    //country
                    var c = _countryService.GetCountryById(x.CountryId);
                    m.CountryName = c != null ? c.Name : "Unavailable";
                    //state
                    var s = _stateProvinceService.GetStateProvinceById(x.StateProvinceId);
                    m.StateProvinceName = s != null ? s.Name : "*";
                    //zip
                    m.Zip = !string.IsNullOrEmpty(x.Zip) ? x.Zip : "*";
                    return m;
                }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = taxRatesModel,
                Total = records.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult AddRateByCountryStateZip(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = new WB_TaxRate
            {
                StoreId = model.AddStoreId,
                TaxCategoryId = model.AddTaxCategoryId,
                CountryId = model.AddCountryId,
                StateProvinceId = model.AddStateProvinceId,
                Zip = model.AddZip,
                Percentage = model.AddPercentage,
            };
            _taxRateService.InsertTaxRate(taxRate);

            return Json(new { Result = true });
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult UpdateRateByCountryStateZip(WB_CountryStateZipModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = _taxRateService.GetTaxRateById(model.Id);
            taxRate.Zip = model.Zip == "*" ? null : model.Zip;
            taxRate.Percentage = model.Percentage;
            _taxRateService.UpdateTaxRate(taxRate);

            return new NullJsonResult();
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult DeleteRateByCountryStateZip(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRate = _taxRateService.GetTaxRateById(id);
            if (taxRate != null)
                _taxRateService.DeleteTaxRate(taxRate);

            return new NullJsonResult();
        }

        #endregion

        #region Tax Category Mapping

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult TaxCategoryMappingList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return ErrorForKendoGridJson("Access denied");

            var records = _taxCategoryMappingService.GetAllTaxCategoryMappings(command.Page - 1, command.PageSize);
            var taxCategoryMappingsModel = records
                .Select(x =>
                {
                    var m = new WB_TaxCategoryMappingModel
                    {
                        Id = x.Id,
                        StoreId = x.StoreId,
                        TaxCategoryId = x.TaxCategoryId,
                        CategoryId = x.CategoryId,
                        Percentage = x.Percentage,
                    };
                    //store
                    var store = _storeService.GetStoreById(x.StoreId);
                    m.StoreName = store != null ? store.Name : "*";
                    //tax category
                    var tc = _taxCategoryService.GetTaxCategoryById(x.TaxCategoryId);
                    m.TaxCategoryName = tc != null ? tc.Name : "";
                    //country
                    var c = _categoryService.GetCategoryById(x.CategoryId);
                    m.CategoryName = c != null ? c.GetFormattedBreadCrumb(_categoryService, ">>") : "Unavailable";
                    return m;
                }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = taxCategoryMappingsModel,
                Total = records.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult AddTaxCategoryMapping(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxCategoryMapping = new WB_TaxCategoryMapping
            {
                StoreId = model.AddStoreId,
                TaxCategoryId = model.AddTaxCategoryId,
                CategoryId = model.AddCategoryId,
                Percentage = model.AddPercentage,
            };
            _taxCategoryMappingService.InsertTaxCategoryMapping(taxCategoryMapping);

            return Json(new { Result = true });
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult UpdateTaxCategoryMapping(WB_TaxCategoryMappingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxCategoryMapping = _taxCategoryMappingService.GetTaxCategoryMappingById(model.Id);
            taxCategoryMapping.CategoryId = model.CategoryId;
            taxCategoryMapping.Percentage = model.Percentage;
            taxCategoryMapping.TaxCategoryId = model.TaxCategoryId;
            _taxCategoryMappingService.UpdateTaxCategoryMapping(taxCategoryMapping);

            return new NullJsonResult();
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult DeleteTaxCategoryMapping(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxCategoryMapping = _taxCategoryMappingService.GetTaxCategoryMappingById(id);
            if (taxCategoryMapping != null)
                _taxCategoryMappingService.DeleteTaxCategoryMapping(taxCategoryMapping);

            return new NullJsonResult();
        }


        #endregion
    }
}