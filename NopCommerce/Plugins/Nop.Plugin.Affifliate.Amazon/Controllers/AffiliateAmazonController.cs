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
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Core.Data;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Core;
using Nop.Plugin.Affiliate.Amazon.Services;
using Nop.Plugin.Affiliate.Amazon.Models;
using Nop.Core.Plugins;
using Nop.Plugin.Affiliate.Amazon.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Affiliate.CategoryMap.Services;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Affiliate.Amazon.Controllers
{
    [AdminAuthorize]
    public class AffiliateAmazonController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryAmazonService _categoryAmazonService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IProductMappingService _productMappingService;
        private readonly ICategoryMappingService _categoryMappingService;
        private readonly IPictureService _pictureService;
        private readonly ICurrencyService _currencyService;


        private readonly IAffiliateAmazonImportManager _importManager;
        private readonly IAmazonService _amazonService;
        private readonly IProductAmazonService _productAmazonService;

        private readonly IRepository<CategoryMapping> _categoryMapRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductMapping> _productMappingRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<CategoryAmazon> _categoryAmazonRepo;

        private readonly IScheduleTaskService _schedualTaskService;
        public AffiliateAmazonController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            ICategoryAmazonService categoryAmazonService,
            ICategoryService categoryService,
            IProductService productService,
            IProductMappingService productMappingService,
            ICategoryMappingService categoryMappingService,
            IPictureService pictureService,
            ICurrencyService currencyService,

            IAffiliateAmazonImportManager importManager,
            IAmazonService amazonService,
            IProductAmazonService productAmazonService,
            IRepository<CategoryMapping> categoryMapRepo,
            IRepository<Product> productRepo,
            IRepository<ProductMapping> productMappingRepo,
            IRepository<Category> categoryRepo,
            IRepository<CategoryAmazon> categoryAmazonRepo,
            IScheduleTaskService schedualTaskService,
            IPluginFinder pluginFinder)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._workContext = workContext;
            this._categoryAmazonService = categoryAmazonService;
            this._categoryService = categoryService;
            this._productService = productService;
            this._productMappingRepo = productMappingRepo;
            this._productMappingService = productMappingService;
            this._categoryMappingService = categoryMappingService;
            this._pictureService = pictureService;
            this._currencyService = currencyService;
            this._categoryMapRepo = categoryMapRepo;
            this._productRepo = productRepo;
            this._importManager = importManager;
            this._amazonService = amazonService;
            this._productAmazonService = productAmazonService;

            this._categoryRepo = categoryRepo;
            this._categoryAmazonRepo = categoryAmazonRepo;

            this._schedualTaskService = schedualTaskService;
        }
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            var model = new ConfigurationModel()
            {
                ActiveStoreScopeConfiguration = storeScope,
                Version = amazonSettings.Version,
                Endpoint = amazonSettings.Endpoint,
                Service = amazonSettings.Service,
                Durations = amazonSettings.Durations,
            };
            return View("~/Plugins/Affiliate.Amazon/Views/Configure.cshtml", model);
        }
        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);

            amazonSettings.Version = model.Version;
            amazonSettings.Service = model.Service;
            amazonSettings.Endpoint = model.Endpoint;
            amazonSettings.Durations = model.Durations;
            amazonSettings.Folder = HttpContext.Server.MapPath("~/Plugins/Affiliate.Amazon/XMLFolder");
            model.ActiveStoreScopeConfiguration = storeScope;

            _settingService.SaveSetting(amazonSettings);
            _settingService.ClearCache();
            AffAmazonContext.Resolve();
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
        public ActionResult Task()
        {
            ScheduleTask model = _schedualTaskService.GetTaskByType("Nop.Plugin.Affiliate.Amazon.AffiliateAmazonTask, Nop.Plugin.Affiliate.Amazon") ?? new Nop.Core.Domain.Tasks.ScheduleTask()
            {
                Enabled = true,
                Name = "Product Update",
                Seconds = 86400,
                StopOnError = false,
                Type = "Nop.Plugin.Affiliate.Amazon.AffiliateAmazonTask, Nop.Plugin.Affiliate.Amazon",
                Id = 0
            };
            return View("~/Plugins/Affiliate.Amazon/Views/Task.cshtml", model);
        }
        [HttpPost]
        public ActionResult Task(ScheduleTask model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    _schedualTaskService.InsertTask(model);
                }
                else
                {
                    _schedualTaskService.UpdateTask(model);
                }
            }
            return Task();
        }
        public ActionResult Accounts()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            var list = !string.IsNullOrEmpty(amazonSettings.Accounts) && !string.IsNullOrWhiteSpace(amazonSettings.Accounts) ? JsonConvert.DeserializeObject<List<AffiliateAmazonAccount>>(amazonSettings.Accounts) : new List<AffiliateAmazonAccount>();
            var accounts = list ?? new List<AffiliateAmazonAccount>();
            int stt = 0;
            accounts.ForEach(x =>
            {
                stt++;
                x.Id = stt;
            });
            return Json(accounts, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreateAccount(string AssociateTag, string AccessKeyID, string SecretKey, bool IsActive)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            var list = !string.IsNullOrEmpty(amazonSettings.Accounts) && !string.IsNullOrWhiteSpace(amazonSettings.Accounts) ? JsonConvert.DeserializeObject<List<AffiliateAmazonAccount>>(amazonSettings.Accounts) : new List<AffiliateAmazonAccount>();
            var accounts = list ?? new List<AffiliateAmazonAccount>();
            int stt = 0;
            accounts.ForEach(x =>
            {
                stt++;
                x.Id = stt;
            });
            if (accounts.FirstOrDefault(x => x.AccessKeyID == AccessKeyID) != null)
            {
                accounts.ForEach(x =>
                {
                    if (x.AccessKeyID == AccessKeyID)
                    {
                        x.SecretKey = SecretKey;
                        x.IsActive = IsActive;
                    }
                });
            }
            else
            {
                accounts.Add(new AffiliateAmazonAccount
                {
                    AssociateTag = AssociateTag,
                    AccessKeyID = AccessKeyID,
                    SecretKey = SecretKey,
                    IsActive = IsActive
                });
            }
            amazonSettings.Accounts = JsonConvert.SerializeObject(accounts);
            _settingService.SaveSetting(amazonSettings);
            _settingService.ClearCache();
            AffAmazonContext.Resolve();
            return Json(new { });
        }
        [HttpPost]
        public ActionResult UpdateAccount(int id,string AssociateTag, string AccessKeyID, string SecretKey, bool IsActive)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            var list = !string.IsNullOrEmpty(amazonSettings.Accounts) && !string.IsNullOrWhiteSpace(amazonSettings.Accounts) ? JsonConvert.DeserializeObject<List<AffiliateAmazonAccount>>(amazonSettings.Accounts) : new List<AffiliateAmazonAccount>();
            var accounts = list ?? new List<AffiliateAmazonAccount>();
            int stt = 0;
            accounts.ForEach(x =>
            {
                stt++;
                x.Id = stt;
            });
            if (accounts.FirstOrDefault(x => x.AccessKeyID == AccessKeyID) != null)
            {
                accounts.ForEach(x =>
                {
                    if (x.AccessKeyID == AccessKeyID)
                    {
                        x.SecretKey = SecretKey;
                        x.IsActive = IsActive;
                    }
                });
            }
            
            amazonSettings.Accounts = JsonConvert.SerializeObject(accounts);
            _settingService.SaveSetting(amazonSettings);
            _settingService.ClearCache();
            AffAmazonContext.Resolve();
            return Json(new { });
        }
        [HttpPost]
        public ActionResult DeleteAccount(int id, string AssociateTag, string AccessKeyID, string SecretKey)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            var list = !string.IsNullOrEmpty(amazonSettings.Accounts) && !string.IsNullOrWhiteSpace(amazonSettings.Accounts) ? JsonConvert.DeserializeObject<List<AffiliateAmazonAccount>>(amazonSettings.Accounts) : new List<AffiliateAmazonAccount>();
            var accounts = list ?? new List<AffiliateAmazonAccount>();
            int stt = 0;
            accounts.ForEach(x =>
            {
                stt++;
                x.Id = stt;
            });
            accounts.Remove(accounts.FirstOrDefault(x => x.AccessKeyID == AccessKeyID));
            amazonSettings.Accounts = JsonConvert.SerializeObject(accounts);

            _settingService.SaveSetting(amazonSettings);
            _settingService.ClearCache();
            AffAmazonContext.Resolve();
            return Json(new { });
        }

        public ActionResult Category(CategorySearch model, int pageIndex = 1, int pageSize = 100)
        {
            var categories = _categoryService.GetAllCategories("", 1);
            ViewBag.Categories = categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.GetFormattedBreadCrumb(_categoryService)
            }).ToList();
            ViewBag.CategoryName = model.CategoryName;
            return View("~/Plugins/Affiliate.Amazon/Views/Category.cshtml", model);
        }
        [HttpPost]
        public ActionResult ReadCategory(DataSourceRequest command, string BrowseNodeId = "", string CategoryName = "", int CompareType = 1)
        {
            var model = new CategorySearch
            {
                BrowseNodeId = BrowseNodeId,
                CategoryName = CategoryName,
                CompareType = CompareType
            };
            var data = _categoryAmazonService.GetAllCategories(model, command.Page, command.PageSize);
            var gridData = new
            {
                data = data,
                totalCount = data.TotalCount,
            };
            var jsonResult = Json(gridData, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
        [HttpPost]
        public ActionResult MapCategory(int CategoryID, int Id, int CategoryMapID)
        {
            bool status = false;
            string message = "";
            if (Request.Form.Count > 0)
            {
                var categoryAmazon = _categoryAmazonRepo.GetById(Id);
                var category = _categoryRepo.GetById(CategoryID);
                if (CategoryMapID == 0)
                {
                    if (CategoryID != 0)
                    {
                        var entity = new CategoryMapping
                        {
                            CategoryId = CategoryID,
                            CategorySourceId = Id,
                            SourceId = 2
                        };
                        _categoryMapRepo.Insert(entity);
                    }
                    var result = new CategoryAmazonModel
                    {
                        BrowseNodeID = categoryAmazon.BrowseNodeID,
                        CategoryID = 0,
                        Id = categoryAmazon.Id,
                        CategoryMapID = 0,
                        Name = categoryAmazon.Name,
                        ParentBrowseNodeID = categoryAmazon.ParentBrowseNodeID,
                    };
                    return Json(result);
                }
                else
                {
                    var entity = _categoryMapRepo.GetById(CategoryMapID);
                    if (entity != null)
                    {
                        if (CategoryID != 0)
                        {
                            entity.CategoryId = CategoryID;
                            _categoryMapRepo.Update(entity);
                            var result = new CategoryAmazonModel
                            {
                                BrowseNodeID = categoryAmazon.BrowseNodeID,
                                CategoryID = category.Id,
                                Id = categoryAmazon.Id,
                                CategoryMapID = entity.Id,
                                Name = categoryAmazon.Name,
                                ParentBrowseNodeID = categoryAmazon.ParentBrowseNodeID,
                            };
                            return Json(result);
                        }
                        else
                        {
                            _categoryMapRepo.Delete(entity);
                            var result = new CategoryAmazonModel
                            {
                                BrowseNodeID = categoryAmazon.BrowseNodeID,
                                CategoryID = 0,
                                Id = categoryAmazon.Id,
                                CategoryMapID = 0,
                                Name = categoryAmazon.Name,
                                ParentBrowseNodeID = categoryAmazon.ParentBrowseNodeID,
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
                var models = JsonConvert.DeserializeObject<List<CategoryAmazonModel>>(Request.Form[0]);

                if (models != null)
                {
                    var model = models[0];
                    _categoryAmazonService.RemoveMapCategory(model.CategoryMapID);
                }
            }
            return Json(new { Status = status, Message = message });
        }
        [HttpPost]
        public ActionResult SyncCategory(string browseNodeID = null)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            _amazonService.SyncCategory(_categoryAmazonRepo,browseNodeID);
            return Json(new { Status = true });
        }
        [HttpPost]
        public ActionResult ImportCategoryFromXlsx()
        {

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportCategoryAmazonFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Affiliate.Amazon.Imported"));
                return RedirectToAction("Category");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Category");
            }

        }

        public ActionResult LoadBrowseNode(int categoryId = 0)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            var model = new List<CategoryAmazon>();
            if (category == null)
                return Json(model, JsonRequestBehavior.AllowGet);
            var categoryMaps = _categoryMappingService.GetAllByCategoryId(2, categoryId);

            if (categoryMaps != null)
            {
                foreach (var cateMap in categoryMaps)
                {
                    var browseNode = _categoryAmazonService.Get(cateMap.CategorySourceId);
                    if (browseNode != null)
                        model.Add(browseNode);
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Product()
        {
            var categories = _categoryService.GetAllCategories("", 1);
            var model = new ProductParameter
            {
                Keywords = "",
                CategoryID = 0,
                SyncProperties = SyncProperties.Attributes | SyncProperties.Images | SyncProperties.Price | SyncProperties.Variations,
                Categories = categories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.GetFormattedBreadCrumb(_categoryService)
                }).ToList(),
            };
            model.Categories.Insert(0, new SelectListItem { Text = "Select Category", Value = "0" });
            return View("~/Plugins/Affiliate.Amazon/Views/Product.cshtml", model);
        }
        [AdminAntiForgery]
        [HttpPost]
        [ActionName("SyncProducts")]
        [FormValueRequired("search")]
        public ActionResult ReadProduct(DataSourceRequest command, ProductParameter model)
        {
            var result = _productAmazonService.GetAllByCategoryId(model, command.Page, command.PageSize);
            var gridData = new
            {
                data = result,
                totalCount = result.TotalCount
            };
            var json = Json(gridData);
            return json;
        }
        [AdminAntiForgery]
        [HttpPost]
        [ActionName("SyncProducts")]
        [FormValueRequired("sync")]
        public ActionResult SyncProducts(ProductParameter model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            if (AffAmazonContext.Instance.Count > 0)
            {
                _amazonService.SyncProducts(_categoryService,_categoryMappingService,_categoryAmazonRepo,storeScope, model.CategoryID, model.Keywords, SyncProperties.All);
            }

            return Json(new { Status = true, Message = "Processing" });
        }
        public ActionResult SyncProduct(int id)
        {
            var model = new ProductParameter
            {
                Id = id,
                SyncProperties = SyncProperties.Price
            };
            return PartialView("~/Plugins/Affiliate.Amazon/Views/SyncProduct.cshtml", model);
        }
        [AdminAntiForgery]
        [HttpPost]
        [ActionName("SyncProduct")]
        [FormValueRequired("sync")]
        public ActionResult SyncProduct(ProductParameter model)
        {
            SyncProperties syncProperties = model.SyncProperties;
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            if (AffAmazonContext.Instance.Count > 0)
            {
                _amazonService.SyncProduct(_productRepo,_productMappingRepo,storeScope, model.Id, syncProperties);
            }
            return Json(new { });
        }
       
        //[AdminAntiForgery]
        //[HttpPost]
        //[ActionName("SyncProducts")]
        //[FormValueRequired("update")]
        //public ActionResult UpdateProducts(ProductParameter model)
        //{
        //    SyncProperties syncProperties = model.SyncProperties;
        //    var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    var amazonSettings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
        //    if (amazonSettings.ListAccounts.Count() > 0)
        //    {
        //        _amazonService.UpdateProducts(storeScope, model.CategoryID, syncProperties);
        //    }

        //    return Json(new { });
        //}
    }
}
