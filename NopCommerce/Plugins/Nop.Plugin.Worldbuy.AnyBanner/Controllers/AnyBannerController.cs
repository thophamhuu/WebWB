using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Plugins;
using Nop.Plugin.Worldbuy.AnyBanner.Domain;
using Nop.Plugin.Worldbuy.AnyBanner.Models;
using Nop.Plugin.Worldbuy.AnyBanner.Services;
using Nop.Plugin.Worldbuy.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Nop.Plugin.Worldbuy.AnyBanner.Controllers
{
    public class AnyBannerController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;

        private readonly ILanguageService _languageService;

        private readonly IWB_AnyBannerService _AnyBannerService;
        private readonly IWB_AnyBannerItemService _AnyBannerItemService;

        private readonly IRepository<WB_AnyBanner> _AnyBannerRepo;
        private readonly IRepository<WB_AnyBannerItem> _AnyBannerItemRepo;
        private readonly IRepository<Setting> _settingRepo;

        public AnyBannerController(
            IWorkContext workContext,
           IStoreContext storeContext,
           IStoreService storeService,
           ISettingService settingService,
           ILocalizationService localizationService,
           ILocalizedEntityService localizedEntityService,

           ILanguageService languageService,

           IWB_AnyBannerService AnyBannerService,
           IWB_AnyBannerItemService AnyBannerItemService,

           IRepository<WB_AnyBanner> AnyBannerRepo,
           IRepository<WB_AnyBannerItem> AnyBannerItemRepo,
           IRepository<Setting> settingRepo,


           IPluginFinder _pluginFinder)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;

            this._AnyBannerService = AnyBannerService;
            this._AnyBannerItemService = AnyBannerItemService;

            this._AnyBannerRepo = AnyBannerRepo;
            this._AnyBannerItemRepo = AnyBannerItemRepo;
            this._settingRepo = settingRepo;

        }
        [AdminAuthorize]
        public ActionResult List()
        {
            ViewBag.WidgetZones = _AnyBannerService.GetWidgetZones().ToList();
            return View("~/Plugins/Worldbuy.AnyBanner/Views/List.cshtml");
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult ReadBanner()
        {
            var model = _AnyBannerService.GetAllModels().ToList();
            if (model != null)
            {
                model.ForEach(x =>
                {
                    string settingKey = string.Format("wb_AnyBannersettings.configs[{0}]", x.Id);
                    var settingValue = _settingService.GetSettingByKey<string>(settingKey);
                    if (!String.IsNullOrEmpty(settingValue) && !String.IsNullOrWhiteSpace(settingValue))
                    {
                        var settings = JsonConvert.DeserializeObject<WB_ColumnOnRowModel>(settingValue);
                        if (settings != null)
                        {
                            x.Settings = settings;
                        }
                    }
                });
            }
            return Json(model);
        }
        [AdminAuthorize]
        public ActionResult Create()
        {
            var model = new WB_AnyBannerModel()
            {
                Settings = new WB_ColumnOnRowModel
                {
                    ColumnPerRow_1280 = 1,
                    ColumnPerRow_1000 = 1,
                    ColumnPerRow_768 = 1,
                    ColumnPerRow_480 = 1,
                },
                Id = 0,
                Name = "",
                WidgetZone = "",
                IsActived = true,
                WidgetZones = _AnyBannerService.GetWidgetZones().ToList()
            };
            return View("~/Plugins/Worldbuy.AnyBanner/Views/Create.cshtml", model);
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult Create(WB_AnyBannerModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            if (ModelState.IsValid)
            {
                var entity = new WB_AnyBanner
                {
                    Name = model.Name,
                    WidgetZone = model.WidgetZone,
                    Id = 0,
                    IsActived = model.IsActived
                };
                try
                {
                    _AnyBannerRepo.Insert(entity);
                    if (entity.Id > 0)
                    {
                        var settings = model.Settings;
                        string settingKey = string.Format("wb_AnyBannersettings.configs[{0}]", entity.Id);
                        string settingValue = JsonConvert.SerializeObject(settings);
                        var setting = new Setting
                        {
                            Name = settingKey,
                            Value = settingValue,
                            StoreId = storeScope
                        };
                        if (_settingService.GetSettingByKey<string>(settingKey) != null)
                        {
                            _settingService.SetSetting<string>(settingKey, settingValue);
                        }
                        else
                        {
                            _settingRepo.Insert(setting);
                        }
                        if (Request["save"] != null)
                        {
                            return RedirectToAction("List");
                        }
                        else if (Request["save-continue"] != null)
                        {
                            return RedirectToAction("Edit", "AnyBanner", new { area = "Admin", id = entity.Id });
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorNotification(ex);
                }
            }
            return Create();
        }
        [AdminAuthorize]
        public ActionResult Edit(int id)
        {
            var model = _AnyBannerService.GetModelById(id);
            if (model == null)
                return HttpNotFound();
            model.WidgetZones = _AnyBannerService.GetWidgetZones().ToList();
            string settingKey = string.Format("wb_AnyBannersettings.configs[{0}]", model.Id);
            var settingValue = _settingService.GetSettingByKey<string>(settingKey);
            if (!String.IsNullOrEmpty(settingValue) && !String.IsNullOrWhiteSpace(settingValue))
            {
                var settings = JsonConvert.DeserializeObject<WB_ColumnOnRowModel>(settingValue);
                if (settings != null)
                {
                    model.Settings = settings;
                }
            }


            return View("~/Plugins/Worldbuy.AnyBanner/Views/Edit.cshtml", model);
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, WB_AnyBannerModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            if (ModelState.IsValid)
            {
                if (id == model.Id)
                {
                    var entity = _AnyBannerRepo.GetById(model.Id);
                    if (entity != null)
                    {
                        entity.Name = model.Name;
                        entity.WidgetZone = model.WidgetZone;
                        entity.IsActived = model.IsActived;
                        try
                        {
                            _AnyBannerRepo.Update(entity);

                            if (entity.Id > 0)
                            {
                                if (model.Settings != null)
                                {
                                    var settings = model.Settings;
                                    string settingKey = string.Format("wb_AnyBannersettings.configs[{0}]", entity.Id);
                                    string settingValue = JsonConvert.SerializeObject(settings);
                                    var setting = new Setting
                                    {
                                        Name = settingKey,
                                        Value = settingValue,
                                        StoreId = storeScope
                                    };
                                    if (_settingService.GetSettingByKey<string>(settingKey) != null)
                                    {
                                        _settingService.SetSetting<string>(settingKey, settingValue);
                                    }
                                    else
                                    {
                                        _settingRepo.Insert(setting);
                                    }
                                }
                            }

                            if (Request["save"] != null)
                            {
                                return RedirectToAction("List");
                            }
                            else if (Request["save-continue"] != null)
                            {
                                return RedirectToAction("Edit", "AnyBanner", new { area = "Admin", id = entity.Id });
                            }
                            else
                            {
                                model = _AnyBannerService.GetModelById(model.Id);
                                return Json(model);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorNotification(ex);
                        }
                    }
                }

            }
            return Edit(id);
        }
        [AdminAuthorize]

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            var entity = _AnyBannerRepo.GetById(Id);
            if (entity != null)
            {
                var items = _AnyBannerItemRepo.Table.Where(x => x.BannerID == entity.Id).ToList();
                if (items != null)
                {
                    _AnyBannerItemRepo.Delete(items);
                }
                _AnyBannerRepo.Delete(entity);
            }
            return Json(new { });
        }
        [AdminAuthorize]

        [HttpPost]
        public ActionResult Items(int bannerId)
        {
            var model = _AnyBannerItemService.GetAllModelsByBannerId(bannerId);
            if (model != null)
            {
                return Json(model);
            }
            return Json(new List<WB_AnyBannerItemModel>());
        }
        [AdminAuthorize]
        public ActionResult Item(int bannerId, int id = 0)
        {
            var model = new WB_AnyBannerItemModel();
            if (id == 0)
            {
                var newItem = new WB_AnyBannerItemModel
                {
                    BannerID = bannerId,
                    IsActived = true,
                };

                model = newItem;
            }
            else
            {
                var item = _AnyBannerItemRepo.GetById(id);
                model.Id = item.Id;
                model.BannerID = item.BannerID;
                model.Title = item.Title;
                model.Url = item.Url;
                model.ImageUrl = item.ImageUrl;
                model.IsActived = item.IsActived;
            }
            return PartialView("~/Plugins/Worldbuy.AnyBanner/Views/_CreateOrUpdate.Item.cshtml", model);
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult SaveItem(WB_AnyBannerItemModel model)
        {
            bool status = false;
            string message = "Error";
            string fileName = "";
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id == 0)
                    {
                        if (Request.Files != null && Request.Files.Count > 0)
                        {
                            for (int i = 0; i < Request.Files.Count; i++)
                            {
                                HttpPostedFileBase file = Request.Files[i];
                                if (file != null && file.ContentLength > 0)
                                {
                                    var folder = "~/Content/Images/Worldbuy.AnyBanner/Images";
                                    var folderPath = Server.MapPath(folder);
                                    if (!System.IO.Directory.Exists(folderPath))
                                        System.IO.Directory.CreateDirectory(folderPath);

                                    var entity = new WB_AnyBannerItem
                                    {
                                        BannerID = model.BannerID,
                                        Id = 0,
                                        IsActived = model.IsActived,
                                        Title = model.Title,
                                        Url = model.Url,
                                        Alt=model.Alt,
                                        
                                    };
                                    _AnyBannerItemRepo.Insert(entity);
                                    if (entity.Id > 0)
                                    {
                                        fileName = folder + "/" + entity.Id.ToString("000000") + System.IO.Path.GetExtension(file.FileName);
                                        entity.ImageUrl = Url.Content(fileName);
                                        _AnyBannerItemRepo.Update(entity);
                                        string filePath = Server.MapPath(fileName);
                                        file.SaveAs(filePath);
                                    }

                                }
                            }
                        }
                        status = true;
                        message = "Success";
                    }
                    else
                    {
                        var entity = _AnyBannerItemRepo.GetById(model.Id);
                        if (entity != null)
                        {
                            entity.Title = model.Title;
                            entity.Url = model.Url;
                            entity.IsActived = model.IsActived;
                            entity.Alt = model.Alt;
                            entity.Order = model.Order;
                            _AnyBannerItemRepo.Update(entity);
                        }
                        model = _AnyBannerItemService.GetModelById(model.Id);
                        return Json(model);
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    LogException(ex);
                }
            }


            return Json(new { Status = status, Message = message });
        }
        [AdminAuthorize]

        [HttpPost]
        public ActionResult DeleteItem(int Id)
        {
            var entity = _AnyBannerItemRepo.GetById(Id);
            bool status = false;
            if (entity != null)
            {
                string fileName = entity.ImageUrl;
                try
                {
                    _AnyBannerItemRepo.Delete(entity);
                    status = true;
                    if (!String.IsNullOrEmpty(fileName) || !String.IsNullOrWhiteSpace(fileName))
                    {
                        string filePath = Server.MapPath(fileName);
                        System.IO.File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
            }
            return Json(new { Status = status });
        }
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var banners = _AnyBannerService.GetAllModels(widgetZone,true);
            var model = banners.ToList();
            model.ForEach(x =>
            {
                string settingKey = string.Format("wb_AnyBannersettings.configs[{0}]", x.Id);
                var settingValue = _settingService.GetSettingByKey<string>(settingKey);
                if (!String.IsNullOrEmpty(settingValue) && !String.IsNullOrWhiteSpace(settingValue))
                {
                    var settings = JsonConvert.DeserializeObject<WB_ColumnOnRowModel>(settingValue);
                    if (settings != null)
                    {
                        x.Settings = settings;
                    }
                }
            });
            //var model =
            //string settingKey = string.Format("wb_AnyBannersettings.configs[{0}]", model.Id);
            //model.Settings= _settingService.GetSettingByKey<WB_AnyBannerSettingsModel>(settingKey, new WB_AnyBannerSettingsModel(), storeScope);

            return View("~/Plugins/Worldbuy.AnyBanner/Views/PublicInfo.cshtml", model);
        }
    }
}
