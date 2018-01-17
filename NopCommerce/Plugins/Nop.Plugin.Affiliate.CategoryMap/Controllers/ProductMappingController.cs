using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Core.Data;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Web.Framework.Kendoui;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Affiliate.CategoryMap.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Core.Caching;

namespace Nop.Plugin.Affiliate.CategoryMap.Controllers
{
    [AdminAuthorize]
    public class ProductMappingController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        private readonly IRepository<ProductMapping> _productMappingRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IProductService _productService;
        private readonly ICacheManager _cacheManager;
        private readonly ICurrencyService _currencyService;
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";
        public ProductMappingController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IRepository<ProductMapping> productMappingRepository,
            IRepository<Product> productRepository,
            IProductService productService,
            ICacheManager cacheManager,
            ICurrencyService currencyService,
            IPluginFinder pluginFinder)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._workContext = workContext;
            this._productMappingRepository = productMappingRepository;
            this._productRepository = productRepository;
            this._productService = productService;
            this._cacheManager = cacheManager;
            this._currencyService = currencyService;
        }
        public ActionResult LoadUrl(int productId = 0)
        {
            string url = "#";
            if (productId > 0)
            {
                var productMapping = _productMappingRepository.TableNoTracking.FirstOrDefault(x => x.ProductId == productId);
                if (productMapping != null)
                    url = productMapping.ProductSourceLink;
            }
            return Json(url, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LoadShippingDescription(int productId)
        {
            var isAffiliate = false;
            string shippingDescriptions = "";
            if (productId > 0)
            {
                var productMapping = _productMappingRepository.TableNoTracking.FirstOrDefault(x => x.ProductId == productId);
                if (productMapping != null)
                {
                    ProductMappingSettings productMappingSettings = _settingService.LoadSetting<ProductMappingSettings>();
                    isAffiliate = true;
                    shippingDescriptions = productMappingSettings.ShippingDescriptions;
                }
            }
            return Json(new { IsAffiliate = isAffiliate, ShippingDescriptions = shippingDescriptions });
        }
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mappingSettings = _settingService.LoadSetting<ProductMappingSettings>(storeScope);
            var model = new ConfigurationModel()
            {
                ActiveStoreScopeConfiguration = storeScope,
                AdditionalCostPercent = mappingSettings.AdditionalCostPercent,
                ShippingDescriptions = mappingSettings.ShippingDescriptions
            };
            return View("~/Plugins/Affiliate.CategoryMap/Views/Configure.cshtml", model);
        }
        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mappingSettings = _settingService.LoadSetting<ProductMappingSettings>(storeScope);

            mappingSettings.AdditionalCostPercent = model.AdditionalCostPercent;
            model.ActiveStoreScopeConfiguration = storeScope;
            mappingSettings.ShippingDescriptions = model.ShippingDescriptions;

            _settingService.SaveSetting(mappingSettings);

            _settingService.ClearCache();
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
        [HttpPost]
        public ActionResult UpdatePrice()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mappingSettings = _settingService.LoadSetting<ProductMappingSettings>(storeScope);
            var query = _productMappingRepository.TableNoTracking.Where(x => x.ProductId > 0).Select(x => new ProductMappingId
            {
                Id = x.Id,

                ProductId = x.ProductId
            }).OrderByDescending(x => x.Id);
            var count = query.Count();
            var currency = _currencyService.GetCurrencyByCode("USD");
            var currncePrimary = _workContext.WorkingCurrency;

            decimal rate = currncePrimary.Rate / currency.Rate;
            var parallel = Parallel.ForEach(query, new ParallelOptions { MaxDegreeOfParallelism = 100 }, productMappingId =>
            {
                var _productMappingRepository = EngineContext.Current.Resolve<IRepository<ProductMapping>>();
                var item = _productMappingRepository.GetById(productMappingId.Id);
                Thread.Sleep(200);
                if (item.ProductId > 0)
                {
                    try
                    {
                        var _productRepo = EngineContext.Current.Resolve<IRepository<Product>>();
                        var product = _productRepo.GetById(item.ProductId);
                        if (product != null)
                        {
                            var price = product.Price;
                            var itemPrice = item.Price * rate;

                            var oldPercent = 0M;
                            if (itemPrice > 0)
                            {
                                oldPercent = 100 % (price - itemPrice) / itemPrice;
                                var oldPrice = product.OldPrice / (1 + oldPercent / 100);

                                product.OldPrice = 0;
                                product.Price = item.Price * (1 + mappingSettings.AdditionalCostPercent / 100) * rate;
                                product.OldPrice = oldPrice * (1 + mappingSettings.AdditionalCostPercent / 100) * rate;
                                if (product.Price == 0)
                                    product.DisableBuyButton = true;

                                var attrCombinations = product.ProductAttributeCombinations.ToList();
                                if (attrCombinations != null && attrCombinations.Count > 0)
                                {
                                    attrCombinations.ForEach(x =>
                                    {
                                        var var = item.Variations.FirstOrDefault(v => v.Sku == x.Sku);
                                        if (var != null)
                                        {
                                            x.OverriddenPrice = var.Price * (1 + mappingSettings.AdditionalCostPercent / 100) * rate;
                                            var _productCombination = EngineContext.Current.Resolve<IRepository<ProductAttributeCombination>>();
                                            _productCombination.Update(x);
                                        }
                                        //decimal oldPriceCombination = x.OverriddenPrice.HasValue ? x.OverriddenPrice.Value / (1 + oldPercent) : 0;
                                        //x.OverriddenPrice = oldPriceCombination * (1 + mappingSettings.AdditionalCostPercent / 100) * rate;
                                        //var _productCombination = EngineContext.Current.Resolve<IRepository<ProductAttributeCombination>>();
                                        //_productCombination.Update(x);
                                        //product.DisableBuyButton = false;
                                    });

                                }
                                _productRepo.Update(product);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var logger = EngineContext.Current.Resolve<ILogger>();
                        logger.Error(ex.Message, ex);
                    }
                }
            });
            if (parallel.IsCompleted)
            {
                _cacheManager.RemoveByPattern(PRODUCTS_PATTERN_KEY);
            }
            return Json(new { Status = true });
        }

        private decimal Round(decimal d, int decimals)
        {
            if (decimals >= 0) return decimal.Round(d, decimals);

            decimal n = (decimal)Math.Pow(10, -decimals);
            return decimal.Round(d / n, 0) * n;
        }

        public class ProductMappingId
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            //public decimal Price { get; set; }
        }
    }
}
