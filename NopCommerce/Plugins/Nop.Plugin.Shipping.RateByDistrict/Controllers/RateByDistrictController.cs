using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using Nop.Plugin.Shipping.RateByDistrict.Services;
using Nop.Plugin.Shipping.RateByDistrict.Models;
using Nop.Plugin.Shipping.RateByDistrict.Domain;
using Nop.Services.Catalog;

namespace Nop.Plugin.Shipping.RateByDistrict.Controllers
{
    [AdminAuthorize]
    public class RateByDistrictController : BasePluginController
    {
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly RateByDistrictSettings _rateByDistrictSettings;
        private readonly IStoreService _storeService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShippingByDistrictService _shippingByDistrictService;
        private readonly IShippingByProductTypeService _shippingByProductTypeService;
        private readonly IShippingByCategoryService _shippingByCategoryService;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;

        private readonly ICategoryService _categoryService;
        public RateByDistrictController(IShippingService shippingServicee,
            ISettingService settingService,
            IPermissionService permissionService,
            IStoreService storeService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShippingByDistrictService shippingByDistrictService,
            IShippingByProductTypeService shippingByProductTypeService,
            IShippingByCategoryService shippingByCategoryService,
            ILocalizationService localizationService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IMeasureService measureService,
            MeasureSettings measureSettings,
            RateByDistrictSettings rateByDistrictSettings,
            ICategoryService categoryService
            )
        {
            this._shippingService = shippingServicee;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._rateByDistrictSettings = rateByDistrictSettings;
            this._storeService = storeService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._shippingByDistrictService = shippingByDistrictService;
            this._shippingByProductTypeService = shippingByProductTypeService;
            this._shippingByCategoryService = shippingByCategoryService;
            this._localizationService = localizationService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._categoryService = categoryService;
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
            var model = new ConfigurationModel
            {
                ShippingByDistrictEnabled = _rateByDistrictSettings.ShippingByDistrictEnabled
            };
            return View("~/Plugins/Shipping.RateByDistrict/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            //save settings

            _settingService.SaveSetting(_rateByDistrictSettings);

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult SaveMode(bool value)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            //save settings
            _rateByDistrictSettings.ShippingByDistrictEnabled = value;
            _settingService.SaveSetting(_rateByDistrictSettings);

            return Json(new
            {
                Result = true
            }, JsonRequestBehavior.AllowGet);
        }

        #region Fixed rate

        [HttpPost]
        public ActionResult FixedShippingRateList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return ErrorForKendoGridJson("Access denied");

            var rateModels = new List<FixedRateModel>();
            foreach (var shippingMethod in _shippingService.GetAllShippingMethods())
                rateModels.Add(new FixedRateModel
                {
                    ShippingMethodId = shippingMethod.Id,
                    ShippingMethodName = shippingMethod.Name,
                    Rate = GetFixedShippingRateValue(shippingMethod.Id)
                });

            var gridModel = new DataSourceResult
            {
                Data = rateModels,
                Total = rateModels.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult UpdateFixedShippingRate(FixedRateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var shippingMethodId = model.ShippingMethodId;
            var rate = model.Rate;

            _settingService.SetSetting(string.Format("ShippingRateComputationMethod.RateByDistrict.Rate.ShippingMethodId{0}", shippingMethodId), rate);

            return new NullJsonResult();
        }

        [NonAction]
        protected decimal GetFixedShippingRateValue(int shippingMethodId)
        {
            var rate = _settingService.GetSettingByKey<decimal>(string.Format("ShippingRateComputationMethod.RateByDistrict.Rate.ShippingMethodId{0}", shippingMethodId));
            return rate;
        }

        #endregion

        #region Rate by District

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult RateByDistrictList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return ErrorForKendoGridJson("Access denied");

            var records = _shippingByDistrictService.GetAll(command.Page - 1, command.PageSize);
            var sbdModel = records.Select(x =>
            {
                var m = new ShippingByDistrictModel
                {
                    Id = x.Id,
                    StoreId = x.StoreId,
                    ShippingMethodId = x.ShippingMethodId,
                    CountryId = x.CountryId,
                    AdditionalFixedCost = x.AdditionalFixedCost,
                };
                //shipping method
                var shippingMethod = _shippingService.GetShippingMethodById(x.ShippingMethodId);
                m.ShippingMethodName = (shippingMethod != null) ? shippingMethod.Name : "Unavailable";
                //store
                var store = _storeService.GetStoreById(x.StoreId);
                m.StoreName = (store != null) ? store.Name : "*";
                //country
                var c = _countryService.GetCountryById(x.CountryId);
                m.CountryName = (c != null) ? c.Name : "*";
                //state
                var s = _stateProvinceService.GetStateProvinceById(x.StateProvinceId);
                m.StateProvinceName = (s != null) ? s.Name : "*";
                //zip
                m.Zip = (!String.IsNullOrEmpty(x.Zip)) ? x.Zip : "*";

                var htmlSb = new StringBuilder("<div>");
                htmlSb.AppendFormat("{0}: {1}", _localizationService.GetResource("Plugins.Shipping.RateByDistrict.Fields.AdditionalFixedCost"), m.AdditionalFixedCost);

                htmlSb.Append("</div>");
                m.DataHtml = htmlSb.ToString();

                return m;
            })
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = sbdModel,
                Total = records.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult AddRateByDistrictPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });

            var model = new ShippingByDistrictModel
            {
                PrimaryStoreCurrencyCode =
                    _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode,
            };

            var shippingMethods = _shippingService.GetAllShippingMethods();
            if (!shippingMethods.Any())
                return Content("No shipping methods can be loaded");

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });
            //shipping methods
            foreach (var sm in shippingMethods)
                model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString() });
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            var countries = _countryService.GetAllCountries(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //states
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });

            return View("~/Plugins/Shipping.RateByDistrict/Views/AddRateByDistrictPopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult AddRateByDistrictPopup(string btnId, string formId, ShippingByDistrictModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });

            var sbd = new ShippingByDistrictRecord
            {
                StoreId = model.StoreId,
                CountryId = model.CountryId,
                StateProvinceId = model.StateProvinceId,
                Zip = model.Zip == "*" ? null : model.Zip,
                ShippingMethodId = model.ShippingMethodId,
                AdditionalFixedCost = model.AdditionalFixedCost,
            };
            _shippingByDistrictService.InsertShippingByDistrictRecord(sbd);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View("~/Plugins/Shipping.RateByDistrict/Views/AddRateByDistrictPopup.cshtml", model);
        }

        public ActionResult EditRateByDistrictPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });

            var sbd = _shippingByDistrictService.GetById(id);
            if (sbd == null)
                //no record found with the specified id
                return RedirectToAction("Configure");

            var model = new ShippingByDistrictModel
            {
                Id = sbd.Id,
                StoreId = sbd.StoreId,
                CountryId = sbd.CountryId,
                StateProvinceId = sbd.StateProvinceId,
                Zip = sbd.Zip,
                ShippingMethodId = sbd.ShippingMethodId,
                AdditionalFixedCost = sbd.AdditionalFixedCost,
                PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode,
            };

            var shippingMethods = _shippingService.GetAllShippingMethods();
            if (!shippingMethods.Any())
                return Content("No shipping methods can be loaded");

            var selectedStore = _storeService.GetStoreById(sbd.StoreId);
            var selectedShippingMethod = _shippingService.GetShippingMethodById(sbd.ShippingMethodId);
            var selectedCountry = _countryService.GetCountryById(sbd.CountryId);
            var selectedState = _stateProvinceService.GetStateProvinceById(sbd.StateProvinceId);
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });
            //warehouses
            foreach (var sm in shippingMethods)
                model.AvailableShippingMethods.Add(new SelectListItem { Text = sm.Name, Value = sm.Id.ToString(), Selected = (selectedShippingMethod != null && sm.Id == selectedShippingMethod.Id) });
            //countries
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            var countries = _countryService.GetAllCountries(showHidden: true);
            foreach (var c in countries)
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (selectedCountry != null && c.Id == selectedCountry.Id) });
            //states
            var states = selectedCountry != null ? _stateProvinceService.GetStateProvincesByCountryId(selectedCountry.Id, showHidden: true).ToList() : new List<StateProvince>();
            model.AvailableStates.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var s in states)
                model.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (selectedState != null && s.Id == selectedState.Id) });

            return View("~/Plugins/Shipping.RateByDistrict/Views/EditRateByDistrictPopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult EditRateByDistrictPopup(string btnId, string formId, ShippingByDistrictModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });

            var sbd = _shippingByDistrictService.GetById(model.Id);
            if (sbd == null)
                //no record found with the specified id
                return RedirectToAction("Configure");

            sbd.StoreId = model.StoreId;
            sbd.CountryId = model.CountryId;
            sbd.StateProvinceId = model.StateProvinceId;
            sbd.Zip = model.Zip == "*" ? null : model.Zip;
            sbd.ShippingMethodId = model.ShippingMethodId;
            sbd.AdditionalFixedCost = model.AdditionalFixedCost;
            _shippingByDistrictService.UpdateShippingByDistrictRecord(sbd);

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;

            return View("~/Plugins/Shipping.RateByDistrict/Views/EditRateByDistrictPopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult DeleteRateByDistrict(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var sbd = _shippingByDistrictService.GetById(id);
            if (sbd != null)
                _shippingByDistrictService.DeleteShippingByDistrictRecord(sbd);

            return new NullJsonResult();
        }

        public ActionResult ProductTypes()
        {
            return View("~/Plugins/Shipping.RateByDistrict/Views/ProductTypes.cshtml");
        }
        [HttpPost]
        [AdminAntiForgery]
        public ActionResult ProductTypes(DataSourceRequest command)
        {
            var result = _shippingByProductTypeService.GetAll(command.Page - 1, command.PageSize);
            var model = result.Select(x => new ShippingByProductTypeModel
            {
                Id = x.Id,
                StoreId = x.StoreId,
                ProductTypeName = x.ProductTypeName,
                AdditionalFixedCost = x.AdditionalFixedCost,

            }).ToList();
            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = result.TotalCount
            };
            return Json(gridModel);
        }
        public ActionResult ProductType(string btnId, string formId, int Id)
        {
            var model = new ShippingByProductTypeModel();
            if (Id == 0)
            {
                model = new ShippingByProductTypeModel
                {
                    Id = 0,
                    AdditionalFixedCost = 0,
                    ProductTypeName = "",
                    StoreId = 0,
                };
            }
            else
            {
                var entity = _shippingByProductTypeService.GetById(Id);
                if (entity == null)
                    return HttpNotFound();
                model = new ShippingByProductTypeModel
                {
                    Id = entity.Id,
                    AdditionalFixedCost = entity.AdditionalFixedCost,
                    ProductTypeName = entity.ProductTypeName,
                    StoreId = entity.StoreId,
                };
            }
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            var selectedStore = _storeService.GetStoreById(model.StoreId);
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });
            return View("~/Plugins/Shipping.RateByDistrict/Views/RateByProductTypePopup.cshtml", model);
        }
        [HttpPost]
        [AdminAntiForgery]
        [ActionName("ProductType")]
        [FormValueRequired("save")]
        public ActionResult ProductType(string btnId, string formId, int Id, ShippingByProductTypeModel model)
        {
            if (ModelState.IsValid)
            {
                if (Id != model.Id)
                    return HttpNotFound();

                var entity = _shippingByProductTypeService.GetById(Id) ?? new ShippingByProductTypeRecord
                {
                    StoreId = 0,
                    AdditionalFixedCost = 0,
                    Id = 0,
                    ProductTypeName = ""
                };
                entity.AdditionalFixedCost = model.AdditionalFixedCost;
                entity.ProductTypeName = model.ProductTypeName;
                entity.StoreId = model.StoreId;
                if (Id == 0)
                {
                    _shippingByProductTypeService.InsertShippingByProductTypeRecord(entity);
                }
                else
                {
                    _shippingByProductTypeService.UpdateShippingByProductTypeRecord(entity);
                }
                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
            }
            var selectedStore = _storeService.GetStoreById(model.StoreId);
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });
            return View("~/Plugins/Shipping.RateByDistrict/Views/RateByProductTypePopup.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        [ActionName("ProductType")]
        [FormValueRequired("delete")]
        public ActionResult DeleteProductType(string btnId, string formId, int Id, ShippingByProductTypeModel model)
        {
            if (ModelState.IsValid)
            {
                if (Id != model.Id)
                    return HttpNotFound();

                var entity = _shippingByProductTypeService.GetById(Id);
                if (entity != null)
                {
                    _shippingByProductTypeService.DeleteShippingByProductTypeRecord(entity);
                }
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            var selectedStore = _storeService.GetStoreById(model.StoreId);
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });
            return View("~/Plugins/Shipping.RateByDistrict/Views/RateByProductTypePopup.cshtml", model);
        }

        public ActionResult Categories()
        {
            return View("~/Plugins/Shipping.RateByDistrict/Views/Categories.cshtml");
        }
        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Categories(DataSourceRequest command)
        {
            var result = _shippingByCategoryService.GetAll(0, command.Page - 1, command.PageSize);
            var model = new List<ShippingByCategoryModel>();
            if (result.Count > 0)
            {
                result.ToList().ForEach(x =>
                {
                    var category = _categoryService.GetCategoryById(x.CategoryId);
                    string categoryName = "";
                    if (category != null)
                    {
                        categoryName = category.GetFormattedBreadCrumb(_categoryService);
                    }
                    var type = _shippingByProductTypeService.GetById(x.ProductTypeId);
                    model.Add(new ShippingByCategoryModel
                    {
                        Id = x.Id,
                        StoreId = x.StoreId,
                        CategoryId = x.CategoryId,
                        ProductTypeId = x.ProductTypeId,
                        CategoryName = categoryName,
                        ProductTypeName = type != null ? type.ProductTypeName : "",
                        AdditionalFixedCost = x.AdditionalFixedCost
                    });
                });
            }
            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = result.TotalCount
            };
            return Json(gridModel);
        }

        public ActionResult Category(string btnId, string formId, int Id)
        {
            var model = new ShippingByCategoryModel();
            if (Id == 0)
            {
                model = new ShippingByCategoryModel
                {
                    Id = 0,
                    AdditionalFixedCost = 0,
                    ProductTypeName = "",
                    StoreId = 0,
                    ProductTypeId = 0,
                    CategoryId = 0,
                    CategoryName = "",
                    StoreName = "",
                };
            }
            else
            {
                var entity = _shippingByCategoryService.GetById(Id);
                if (entity == null)
                    return HttpNotFound();
                model = new ShippingByCategoryModel
                {
                    Id = entity.Id,
                    AdditionalFixedCost = entity.AdditionalFixedCost,
                    StoreId = entity.StoreId,
                    ProductTypeId = entity.ProductTypeId,
                    CategoryId = entity.CategoryId,
                };

            }
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            var selectedStore = _storeService.GetStoreById(model.StoreId);
            var selectedType = _shippingByProductTypeService.GetById(model.ProductTypeId);
            var selectedCate = _categoryService.GetCategoryById(model.CategoryId);
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });

            model.AvailableProductTypes.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var productType in _shippingByProductTypeService.GetAll())
                model.AvailableProductTypes.Add(new SelectListItem { Text = productType.ProductTypeName, Value = productType.Id.ToString(), Selected = (selectedType != null && selectedType.Id == productType.Id) });

            model.AvailableCategories.Add(new SelectListItem { Text = "*", Value = "0" });
            var categories = _categoryService.GetAllCategories();

            foreach (var category in _categoryService.GetAllCategories())
            {
                model.AvailableCategories.Add(new SelectListItem
                {
                    Text = category.GetFormattedBreadCrumb(_categoryService),
                    Value = category.Id.ToString(),
                    Selected = (selectedCate != null && selectedCate.Id == category.Id)
                });
            }
            return View("~/Plugins/Shipping.RateByDistrict/Views/RateByCategoryPopup.cshtml", model);
        }
        [HttpPost]
        [AdminAntiForgery]
        [ActionName("Category")]
        [FormValueRequired("save")]
        public ActionResult Category(string btnId, string formId, int Id, ShippingByCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                if (Id != model.Id)
                    return HttpNotFound();

                var entity = _shippingByCategoryService.GetById(Id) ?? new ShippingByCategoryRecord
                {
                    StoreId = 0,
                    AdditionalFixedCost = 0,
                    Id = 0,
                };
                entity.AdditionalFixedCost = model.AdditionalFixedCost;
                entity.CategoryId = model.CategoryId;
                entity.ProductTypeId = model.ProductTypeId;
                entity.StoreId = model.StoreId;
                if (Id == 0)
                {
                    _shippingByCategoryService.InsertShippingByCategoryRecord(entity);
                }
                else
                {
                    _shippingByCategoryService.UpdateShippingByCategoryRecord(entity);
                }
                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
            }
            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            var selectedStore = _storeService.GetStoreById(model.StoreId);
            var selectedType = _shippingByProductTypeService.GetById(model.ProductTypeId);
            var selectedCate = _categoryService.GetCategoryById(model.CategoryId);
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });

            model.AvailableProductTypes.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var productType in _shippingByProductTypeService.GetAll())
                model.AvailableProductTypes.Add(new SelectListItem { Text = productType.ProductTypeName, Value = productType.Id.ToString(), Selected = (selectedType != null && selectedType.Id == productType.Id) });

            model.AvailableCategories.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var category in _categoryService.GetAllCategories())
            {
                model.AvailableCategories.Add(new SelectListItem
                {
                    Text = category.GetFormattedBreadCrumb(_categoryService),
                    Value = category.Id.ToString(),
                    Selected = (selectedCate != null && selectedCate.Id == category.Id)
                });


            }

            return View("~/Plugins/Shipping.RateByDistrict/Views/RateByCategoryPopup.cshtml", model);
        }
        [HttpPost]
        [AdminAntiForgery]
        [ActionName("Category")]
        [FormValueRequired("delete")]
        public ActionResult DeleteCategory(string btnId, string formId, int Id, ShippingByCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                if (Id != model.Id)
                    return HttpNotFound();

                var entity = _shippingByCategoryService.GetById(Id);
                if (entity != null)
                {
                    _shippingByCategoryService.DeleteShippingByCategoryRecord(entity);
                }
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            var selectedStore = _storeService.GetStoreById(model.StoreId);
            model.AvailableStores.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString(), Selected = (selectedStore != null && store.Id == selectedStore.Id) });
            return View("~/Plugins/Shipping.RateByDistrict/Views/RateByCategoryPopup.cshtml", model);
        }

        #endregion
    }
}
