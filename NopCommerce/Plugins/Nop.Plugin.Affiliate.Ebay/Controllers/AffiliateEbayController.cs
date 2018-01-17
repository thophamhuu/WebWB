using Newtonsoft.Json;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.Affiliate.CategoryMap;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Affiliate.CategoryMap.Services;
using Nop.Plugin.Affiliate.Ebay.Domain;
using Nop.Plugin.Affiliate.Ebay.Ebay.Finding;
using Nop.Plugin.Affiliate.Ebay.Models;
using Nop.Plugin.Affiliate.Ebay.Models.SearchOutput;
using Nop.Plugin.Affiliate.Ebay.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Affiliate.Ebay.Controllers
{
    [AdminAuthorize]
    public partial class AffiliateEbayController : BasePluginController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IAffiliateEbayService _affiliateEbayService;
        private readonly IRepository<CategoryMapping> _categoryMappingRecord;
        private readonly IProductMappingService _productMappingService;
        private readonly ICacheManager _cacheManager;
        private readonly ISpecificationAttributeService _specificationAttributeService;

        #endregion

        #region Constructors

        public AffiliateEbayController(IWorkContext workContext, IStoreContext storeContext, IStoreService storeService, ISettingService settingService, ILocalizationService localizationService,
            ICategoryService categoryService, IProductService productService, IPictureService pictureService, IAffiliateEbayService affiliateEbayService, ICacheManager cacheManager,
            IRepository<CategoryMapping> categoryMappingRecord, IProductMappingService productMappingService, ISpecificationAttributeService specificationAttributeService)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._workContext = workContext;
            this._categoryService = categoryService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._affiliateEbayService = affiliateEbayService;
            this._categoryMappingRecord = categoryMappingRecord;
            this._cacheManager = cacheManager;
            this._productMappingService = productMappingService;
            this._specificationAttributeService = specificationAttributeService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void SaveCategoryMappings(Product product, List<int> SelectedCategoryIds)
        {
            var existingProductCategories = _categoryService.GetProductCategoriesByProductId(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    _categoryService.DeleteProductCategory(existingProductCategory);

            //add categories
            foreach (var categoryId in SelectedCategoryIds)
                if (existingProductCategories.FindProductCategory(product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = _categoryService.GetProductCategoriesByCategoryId(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    _categoryService.InsertProductCategory(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
        }

        [NonAction]
        protected decimal Round(decimal d, int decimals)
        {
            if (decimals >= 0) return decimal.Round(d, decimals);

            decimal n = (decimal)Math.Pow(10, -decimals);
            return decimal.Round(d / n, 0) * n;
        }

        #endregion

        #region Methods
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ebaySettings = _settingService.LoadSetting<ConfigurationModel>(storeScope);
            var model = new ConfigurationModel()
            {
                ActiveStoreScopeConfiguration = storeScope,
                AppID = ebaySettings.AppID,
                DevID = ebaySettings.DevID,
                Token = ebaySettings.Token,
                CertID = ebaySettings.CertID
            };
            return View("~/Plugins/Affiliate.Ebay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public ActionResult Configure(ConfigurationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ebaySettings = _settingService.LoadSetting<ConfigurationModel>(storeScope);

            //ebaySettings.SiteID = model.SiteID;
            ebaySettings.Token = model.Token;
            //ebaySettings.Version = model.Version;
            ebaySettings.AppID = model.AppID;
            model.ActiveStoreScopeConfiguration = storeScope;

            _settingService.SaveSetting(ebaySettings);

            _settingService.ClearCache();
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public ActionResult CallApi()
        {
            var model = new ConfigurationModel();
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategory.Add(c);
            return View("~/Plugins/Affiliate.Ebay/Views/CallApi.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public async Task<ActionResult> CallApi(ConfigurationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mappingSettings = _settingService.LoadSetting<ProductMappingSettings>(storeScope);

            var tokenebay = EbayExtensions.GetToken();

            var categoryWorldBuy = _categoryMappingRecord.Table.Where(u => u.CategoryId == model.CategoryId && u.SourceId == (int)Source.Ebay).ToList();

            if (categoryWorldBuy != null)
            {
                foreach (var cateIds in categoryWorldBuy)
                {
                    var clientapi1 = new HttpClient();
                    clientapi1.BaseAddress = new Uri("https://api.ebay.com/");
                    clientapi1.DefaultRequestHeaders.Clear();
                    clientapi1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientapi1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenebay);
                    clientapi1.Timeout = TimeSpan.FromMinutes(60);

                    var getCategoryId = _affiliateEbayService.Get(cateIds.CategorySourceId).EbayId.ToString();

                    string str = "buy/browse/v1/item_summary/search?limit=200&category_ids=" + getCategoryId;
                    if (!string.IsNullOrEmpty(model.KeyWord))
                        str = str + "&q=" + model.KeyWord;

                    HttpResponseMessage Res1 = await clientapi1.GetAsync(str);
                    if (Res1.IsSuccessStatusCode)
                    {
                        var EmpResponse1 = Res1.Content.ReadAsStringAsync().Result;
                        var result1 = JsonConvert.DeserializeObject<SearchOutput>(EmpResponse1);

                        int temp = int.Parse(result1.total);
                        int value = 0;
                        while (temp > 0)
                        {
                            str = str + "&offset=" + value;
                            HttpResponseMessage Res2 = await clientapi1.GetAsync(str);
                            if(Res2.IsSuccessStatusCode)
                            {
                                var EmpResponse2 = Res2.Content.ReadAsStringAsync().Result;
                                var result2 = JsonConvert.DeserializeObject<SearchOutput>(EmpResponse2);

                                if (result2.itemSummaries != null)
                                {
                                    foreach (var item in result2.itemSummaries)
                                    {
                                        var checkProduct = _affiliateEbayService.GetProductBySourceId(productSourceId: item.itemId, source: (int)Source.Ebay);
                                        if (checkProduct.Id == 0)
                                        {
                                            var clientapi = new HttpClient();
                                            clientapi.BaseAddress = new Uri("https://api.ebay.com/");
                                            clientapi.DefaultRequestHeaders.Clear();
                                            clientapi.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                            clientapi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenebay);
                                            clientapi.Timeout = TimeSpan.FromMinutes(60);

                                            HttpResponseMessage Res = await clientapi.GetAsync("buy/browse/v1/item/" + item.itemId);

                                            if (Res.IsSuccessStatusCode)
                                            {
                                                var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                                                var result = JsonConvert.DeserializeObject<ProductModelApi>(EmpResponse);

                                                if (result.price != null)
                                                {
                                                    var price = Convert.ToDecimal(result.price.value);
                                                    var product = new Product();
                                                    product.Name = result.title;
                                                    var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
                                                    product.Price = Round(currencyService.ConvertToPrimaryStoreCurrency(price * (1 + mappingSettings.AdditionalCostPercent / 100), currencyService.GetCurrencyByCode("USD")), -3);
                                                    //if(result.marketingPrice == null)
                                                    //{
                                                    //    product.OldPrice = 0;
                                                    //}
                                                    //else
                                                    //{
                                                    //    if(result.marketingPrice.originalPrice == null)
                                                    //        product.OldPrice = 0;
                                                    //    else
                                                    //        product.OldPrice = Convert.ToDecimal(result.marketingPrice.originalPrice.value);
                                                    //}
                                                    product.ShortDescription = result.shortDescription;
                                                    product.FullDescription = result.description;

                                                    product.VisibleIndividually = true;
                                                    product.AllowCustomerReviews = true;
                                                    product.UnlimitedDownloads = true;
                                                    product.MaxNumberOfDownloads = 10;
                                                    product.RecurringCycleLength = 100;
                                                    product.RecurringTotalCycles = 10;
                                                    product.RentalPriceLength = 1;
                                                    product.IsShipEnabled = true;
                                                    product.NotifyAdminForQuantityBelow = 1;
                                                    product.StockQuantity = 1000;
                                                    product.OrderMaximumQuantity = 1000;
                                                    product.OrderMinimumQuantity = 1;
                                                    product.CreatedOnUtc = DateTime.UtcNow;
                                                    product.UpdatedOnUtc = DateTime.UtcNow;
                                                    _productService.InsertProduct(product);

                                                    var productMapping = new ProductMapping();
                                                    productMapping.ProductSourceId = item.itemId;
                                                    productMapping.ProductSourceLink = item.itemWebUrl;
                                                    productMapping.SourceId = (int)Source.Ebay;
                                                    productMapping.ProductId = product.Id;
                                                    productMapping.Price = price;
                                                    _productMappingService.InsertProduct(productMapping);

                                                    // Thêm hình chính
                                                    var imageMain = result.image.imageUrl.Split('?')[0];
                                                    System.Drawing.Image imageKey = EbayExtensions.DownloadImage(imageMain);
                                                    if (imageKey != null)
                                                    {
                                                        var contentTypeMain = "";
                                                        var vendorPictureBinaryMain = EbayExtensions.ImageToByte(imageKey);

                                                        var fileExtensionMain = Path.GetExtension(imageMain);
                                                        if (!String.IsNullOrEmpty(fileExtensionMain))
                                                            fileExtensionMain = fileExtensionMain.ToLowerInvariant();
                                                        if (String.IsNullOrEmpty(contentTypeMain))
                                                        {
                                                            switch (fileExtensionMain)
                                                            {
                                                                case ".bmp":
                                                                    contentTypeMain = MimeTypes.ImageBmp;
                                                                    break;
                                                                case ".gif":
                                                                    contentTypeMain = MimeTypes.ImageGif;
                                                                    break;
                                                                case ".jpeg":
                                                                case ".jpg":
                                                                case ".jpe":
                                                                case ".jfif":
                                                                case ".pjpeg":
                                                                case ".pjp":
                                                                    contentTypeMain = MimeTypes.ImageJpeg;
                                                                    break;
                                                                case ".png":
                                                                    contentTypeMain = MimeTypes.ImagePng;
                                                                    break;
                                                                case ".tiff":
                                                                case ".tif":
                                                                    contentTypeMain = MimeTypes.ImageTiff;
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        var pictureMain = _pictureService.InsertPicture(vendorPictureBinaryMain, contentTypeMain, null);
                                                        _productService.InsertProductPicture(new ProductPicture
                                                        {
                                                            PictureId = pictureMain.Id,
                                                            ProductId = product.Id,
                                                            DisplayOrder = 0,
                                                        });
                                                    }

                                                    int display = 1;
                                                    if (result.additionalImages != null)
                                                    {
                                                        foreach (var ite in result.additionalImages)
                                                        {
                                                            var ima = ite.imageUrl.Split('?')[0];
                                                            System.Drawing.Image image = EbayExtensions.DownloadImage(ima);
                                                            if (image != null)
                                                            {
                                                                var contentType = "";
                                                                var vendorPictureBinary = EbayExtensions.ImageToByte(image);

                                                                var fileExtension = Path.GetExtension(ima);
                                                                if (!String.IsNullOrEmpty(fileExtension))
                                                                    fileExtension = fileExtension.ToLowerInvariant();
                                                                if (String.IsNullOrEmpty(contentType))
                                                                {
                                                                    switch (fileExtension)
                                                                    {
                                                                        case ".bmp":
                                                                            contentType = MimeTypes.ImageBmp;
                                                                            break;
                                                                        case ".gif":
                                                                            contentType = MimeTypes.ImageGif;
                                                                            break;
                                                                        case ".jpeg":
                                                                        case ".jpg":
                                                                        case ".jpe":
                                                                        case ".jfif":
                                                                        case ".pjpeg":
                                                                        case ".pjp":
                                                                            contentType = MimeTypes.ImageJpeg;
                                                                            break;
                                                                        case ".png":
                                                                            contentType = MimeTypes.ImagePng;
                                                                            break;
                                                                        case ".tiff":
                                                                        case ".tif":
                                                                            contentType = MimeTypes.ImageTiff;
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                }
                                                                var picture = _pictureService.InsertPicture(vendorPictureBinary, contentType, null);
                                                                _productService.InsertProductPicture(new ProductPicture
                                                                {
                                                                    PictureId = picture.Id,
                                                                    ProductId = product.Id,
                                                                    DisplayOrder = display++,
                                                                });
                                                            }
                                                        }
                                                    }

                                                    //Product specification attributes
                                                    if(result.localizedAspects != null)
                                                    {
                                                        foreach(var attributes in result.localizedAspects)
                                                        {
                                                            var getAttribute = _affiliateEbayService.GetSpecificationAttributeByName(attributes.name);
                                                            var specificationAttribute = new SpecificationAttribute();
                                                            var specificationAttributeOption = new SpecificationAttributeOption();
                                                            if (getAttribute == null)
                                                            {
                                                                specificationAttribute.Name = attributes.name;
                                                                specificationAttribute.DisplayOrder = 0;
                                                                _specificationAttributeService.InsertSpecificationAttribute(specificationAttribute);

                                                                specificationAttributeOption.DisplayOrder = 0;
                                                                specificationAttributeOption.ColorSquaresRgb = null;
                                                                specificationAttributeOption.Name = attributes.value;
                                                                specificationAttributeOption.SpecificationAttributeId = specificationAttribute.Id;
                                                                _specificationAttributeService.InsertSpecificationAttributeOption(specificationAttributeOption);
                                                            }

                                                            var productSpecificationAttribute = new ProductSpecificationAttribute();
                                                            productSpecificationAttribute.AttributeTypeId = (int)SpecificationAttributeType.CustomText;
                                                            if(getAttribute == null)
                                                            {
                                                                productSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOption.Id;
                                                            }
                                                            else
                                                            {
                                                                var options = _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(getAttribute.Id);
                                                                productSpecificationAttribute.SpecificationAttributeOptionId = options.FirstOrDefault().Id;
                                                            }
                                                            productSpecificationAttribute.ProductId = product.Id;
                                                            productSpecificationAttribute.CustomValue = attributes.value;
                                                            productSpecificationAttribute.AllowFiltering = false;
                                                            productSpecificationAttribute.ShowOnProductPage = true;
                                                            productSpecificationAttribute.DisplayOrder = 1;
                                                            _specificationAttributeService.InsertProductSpecificationAttribute(productSpecificationAttribute);
                                                        }
                                                    }

                                                    //categories
                                                    SaveCategoryMappings(product, new List<int>() { model.CategoryId });
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            value = value + 200;
                            temp = temp - 200;
                        }
                    }
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Added"));
            }
            else
            {
                ErrorNotification(_localizationService.GetResource("Plugins.AffiliateEbay.CallApi.Error"));
            }

            return CallApi();
        }

        public ActionResult MapCategory()
        {
            var categories = _categoryService.GetAllCategories();
            ViewBag.Categories = categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.GetFormattedBreadCrumb(_categoryService)
            }).ToList();
            return View("~/Plugins/Affiliate.Ebay/Views/MapCategory.cshtml");
        }

        [HttpPost]
        public ActionResult ReadCategory(CategoryEbayModel data, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var model = _affiliateEbayService.GetAllCategories(pageIndex - 1, pageSize);

            var gridModel = new DataSourceResult
            {
                Data = model,
                Total = model.TotalCount,
            };
            //result.MaxJsonLength = Int32.MaxValue;
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult MappingCategory()
        {
            bool status = false;
            string message = "";
            if (Request.Form.Count > 0)
            {
                var models = JsonConvert.DeserializeObject<List<CategoryEbayModel>>(Request.Form[0]);

                if (models != null)
                {
                    var model = models[0];
                    var category = _categoryService.GetCategoryById(model.CategoryID);
                    var categoryEbay = _affiliateEbayService.Get(model.Id);
                    if (categoryEbay != null && category != null)
                    {
                        var categoryMap = _affiliateEbayService.MapCategory(categoryEbay.Id, category.Id);
                        if (categoryMap.Id > 0)
                        {
                            var result = new CategoryEbayModel
                            {
                                EbayId = categoryEbay.EbayId,
                                CategoryID = category.Id,
                                CategoryMapID = categoryMap.Id,
                                CategoryName = category.Name,
                                Name = categoryEbay.Name,
                                Id = categoryEbay.Id,
                                ParentCategoryId = categoryEbay.ParentCategoryId
                            };
                            return Json(result);
                        }

                    }
                }
            }
            return Json(new { Status = status, Message = message });
        }

        [HttpPost]
        public ActionResult RemoveMapCategory()
        {
            bool status = false;
            string message = "";
            if (Request.Form.Count > 0)
            {
                var models = JsonConvert.DeserializeObject<List<CategoryEbayModel>>(Request.Form[0]);

                if (models != null)
                {
                    var model = models[0];
                    _affiliateEbayService.RemoveMapCategory(model.CategoryMapID);
                }
            }
            return Json(new { Status = status, Message = message });
        }

        [HttpPost]
        public virtual ActionResult ProductList(DataSourceRequest command, ConfigurationModel model)
        {
            var categoryIds = new List<int> { model.CategoryId };
            //include subcategories
            categoryIds.AddRange(GetChildCategoryIds(model.CategoryId));

            var products = _affiliateEbayService.GetAllProduct(categoryIds: categoryIds, pageIndex: command.Page - 1, pageSize: command.PageSize);
            var gridModel = new DataSourceResult();
            gridModel.Data = products.Select(x =>
            {
                var productModel = x.ToModel();
                //little performance optimization: ensure that "FullDescription" is not returned
                productModel.FullDescription = _productMappingService.GetProductMappingByProductId(x.Id).ProductSourceLink;
                //picture
                var defaultProductPicture = _pictureService.GetPicturesByProductId(x.Id, 1).FirstOrDefault();
                productModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true);
                //product type
                productModel.ProductTypeName = x.ProductType.GetLocalizedEnum(_localizationService, _workContext);

                return productModel;
            });
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }

        //public ActionResult ConvertPrice ()
        //{
        //    var productMapping = _productMappingService.GetAllPriceNull();
        //    foreach(var item in productMapping)
        //    {
        //        var product = _productService.GetProductById(item.ProductId);
        //        var price = product.Price;

        //        product.Price = Math.Round((price * (decimal)0.05) + price, 2);
        //        _productService.UpdateProduct(product);

        //        item.Price = price;
        //        _productMappingService.UpdateProductMapping(item);
        //    }
        //    return new EmptyResult();
        //}

        #endregion
    }
}
