using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Plugins;
using Nop.Plugin.Worldbuy.SimpleMenu.Domain;
using Nop.Plugin.Worldbuy.SimpleMenu.Models;
using Nop.Plugin.Worldbuy.SimpleMenu.Services;
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

namespace Nop.Plugin.Worldbuy.SimpleMenu.Controllers
{

    public class SimpleMenuController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;

        private readonly ILanguageService _languageService;

        private readonly IWB_SimpleMenuService _simpleMenuService;
        private readonly IWB_SimpleMenuItemService _simpleMenuItemService;

        private readonly IRepository<WB_SimpleMenu> _simpleMenuRepo;
        private readonly IRepository<WB_SimpleMenuItem> _simpleMenuItemRepo;
        private readonly IRepository<Setting> _settingRepo;

        public SimpleMenuController(
            IWorkContext workContext,
           IStoreContext storeContext,
           IStoreService storeService,
           ISettingService settingService,
           ILocalizationService localizationService,
           ILocalizedEntityService localizedEntityService,

           ILanguageService languageService,

           IWB_SimpleMenuService simpleMenuService,
           IWB_SimpleMenuItemService simpleMenuItemService,

           IRepository<WB_SimpleMenu> simpleMenuRepo,
           IRepository<WB_SimpleMenuItem> simpleMenuItemRepo,
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

            this._simpleMenuService = simpleMenuService;
            this._simpleMenuItemService = simpleMenuItemService;

            this._simpleMenuRepo = simpleMenuRepo;
            this._simpleMenuItemRepo = simpleMenuItemRepo;
            this._settingRepo = settingRepo;

        }
        [AdminAuthorize]
        public ActionResult List()
        {
            return View("~/Plugins/Worldbuy.SimpleMenu/Views/List.cshtml");
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult ReadMenu()
        {
            var model = _simpleMenuService.GetAllModels();
            return Json(model);
        }
        [AdminAuthorize]
        public ActionResult Create()
        {
            var model = new WB_SimpleMenuModel()
            {
                Settings = new WB_SimpleMenuSettingsModel
                {
                    IconToRight = false,
                    UseIconImage = false,
                },
                Id = 0,
                Name = "",
                WidgetZone = "",
                WidgetZones = _simpleMenuService.GetWidgetZones().ToList()
            };
            return View("~/Plugins/Worldbuy.SimpleMenu/Views/Create.cshtml", model);
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult Create(WB_SimpleMenuModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            if (ModelState.IsValid)
            {
                var entity = new WB_SimpleMenu
                {
                    Name = model.Name,
                    WidgetZone = model.WidgetZone,
                    IsActived = model.IsActived
                };
                try
                {
                    _simpleMenuRepo.Insert(entity);
                    if (entity.Id > 0)
                    {
                        if (model.Settings != null)
                        {
                            var settings = model.Settings;
                            string settingKey = string.Format("wb_simplemenusettings.configs[{0}]", entity.Id);
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

                        if (Request["save"] != null)
                        {
                            return RedirectToAction("List");
                        }
                        else if (Request["save-continue"] != null)
                        {
                            return RedirectToAction("Edit", "SimpleMenu", new { area = "Admin", id = entity.Id });
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
            var model = _simpleMenuService.GetModelById(id);
            if (model == null)
                return HttpNotFound();
            model.WidgetZones = _simpleMenuService.GetWidgetZones().ToList();
            string settingKey = string.Format("wb_simplemenusettings.configs[{0}]", model.Id);
            var settingValue = _settingService.GetSettingByKey<string>(settingKey);
            if (!String.IsNullOrEmpty(settingValue) && !String.IsNullOrWhiteSpace(settingValue))
            {
                var settings = JsonConvert.DeserializeObject<WB_SimpleMenuSettingsModel>(settingValue);
                if (settings != null)
                {
                    model.Settings = settings;
                }
            }


            return View("~/Plugins/Worldbuy.SimpleMenu/Views/Edit.cshtml", model);
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult Edit(int id, WB_SimpleMenuModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            if (ModelState.IsValid)
            {
                if (id == model.Id)
                {
                    var entity = _simpleMenuRepo.GetById(model.Id);
                    if (entity != null)
                    {
                        entity.Name = model.Name;
                        entity.WidgetZone = model.WidgetZone;
                        entity.IsActived = model.IsActived;
                        try
                        {
                            _simpleMenuRepo.Update(entity);

                            if (entity.Id > 0)
                            {
                                if (model.Settings != null)
                                {
                                    var settings = model.Settings;
                                    string settingKey = string.Format("wb_simplemenusettings.configs[{0}]", entity.Id);
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

                                if (Request["save"] != null)
                                {
                                    return RedirectToAction("List");
                                }
                                else if (Request["save-continue"] != null)
                                {
                                    return RedirectToAction("Edit", "SimpleMenu", new { area = "Admin", id = entity.Id });
                                }
                                else
                                {
                                    model = _simpleMenuService.GetModelById(model.Id);
                                    return Json(model);
                                }
                            }

                            if (Request["save"] != null)
                            {
                                return RedirectToAction("List");
                            }
                            else if (Request["save-continue"] != null)
                            {
                                return RedirectToAction("Edit", "SimpleMenu", new { area = "Admin", id = entity.Id });
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
            var entity = _simpleMenuRepo.GetById(Id);
            if (entity != null)
            {
                var items = _simpleMenuItemRepo.Table.Where(x => x.MenuID == entity.Id).ToList();
                if (items != null)
                {
                    _simpleMenuItemRepo.Delete(items);
                }
                _simpleMenuRepo.Delete(entity);
            }
            return Json(new { });
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult Items(int menuId)
        {
            var model = _simpleMenuItemService.GetAllModelsByMenuId(menuId);
            if (model != null)
            {
                return Json(model);
            }
            return Json(new List<WB_SimpleMenuItemModel>());
        }
        [AdminAuthorize]
        public ActionResult Item(int menuId, int id = 0)
        {
            var model = new WB_SimpleMenuItemModel();
            if (id == 0)
            {
                var newItem = new WB_SimpleMenuItemModel
                {
                    MenuID = menuId
                };
                newItem.Locales = new List<WB_SimpleMenuItemLocalizedModel>();
                foreach (var lang in _languageService.GetAllLanguages())
                {
                    newItem.Locales.Add(new WB_SimpleMenuItemLocalizedModel
                    {
                        LanguageId = lang.Id,
                    });
                }
                model = newItem;
            }
            else
            {
                var item = _simpleMenuItemRepo.GetById(id);
                model.Id = item.Id;
                model.MenuID = item.MenuID;
                model.Title = item.Title;
                model.Url = item.Url;
                model.Order = item.Order;
                model.IconUrlImage = item.IconUrlImage;
                model.Locales = new List<WB_SimpleMenuItemLocalizedModel>();
                foreach (var lang in _languageService.GetAllLanguages())
                {
                    var title = item.GetLocalized(x => x.Title, lang.Id);
                    model.Locales.Add(new WB_SimpleMenuItemLocalizedModel
                    {
                        LanguageId = lang.Id,
                        Title = title
                    });
                }
            }
            return PartialView("~/Plugins/Worldbuy.SimpleMenu/Views/_CreateOrUpdate.Item.cshtml", model);
        }
        [AdminAuthorize]
        [HttpPost]
        public ActionResult SaveItem(WB_SimpleMenuItemModel model)
        {
            bool status = false;
            string message = "Error";
            string fileName = "";
            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Files != null && Request.Files.Count > 0)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        if (file != null && file.ContentLength > 0)
                        {
                            var folder = "~/Content/Images/Worldbuy.SimpleMenu/Icons";
                            var folderPath = Server.MapPath(folder);
                            if (!System.IO.Directory.Exists(folderPath))
                                System.IO.Directory.CreateDirectory(folderPath);
                            fileName = folder + "/" + file.FileName;
                            string filePath = Server.MapPath(fileName);
                            file.SaveAs(filePath);
                        }
                    }

                    var entity = _simpleMenuItemRepo.GetById(model.Id) ?? new WB_SimpleMenuItem
                    {
                        Id = 0,
                        MenuID = model.MenuID,
                        Title = model.Title,
                        Url = model.Url,
                        Order = model.Order,
                        IconUrlImage = fileName != "" ? Url.Content(fileName) : fileName
                    };
                    if (model.Id == 0)
                        _simpleMenuItemRepo.Insert(entity);
                    else
                    {
                        entity.Title = model.Title;
                        entity.Url = model.Url;
                        entity.Order = model.Order;
                        _simpleMenuItemRepo.Update(entity);
                    }
                    UpdateLocales(entity, model);
                    if (entity.Id > 0)
                    {
                        status = true;
                        message = "Success";
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
            var entity = _simpleMenuItemRepo.GetById(Id);
            bool status = false;
            if (entity != null)
            {
                string fileName = entity.IconUrlImage;
                try
                {
                    _simpleMenuItemRepo.Delete(entity);
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
            var menus = _simpleMenuService.GetAllModels(widgetZone, true);
            var model = menus.ToList();
            model.ForEach(x =>
            {
                string settingKey = string.Format("wb_simplemenusettings.configs[{0}]", x.Id);
                var settingValue = _settingService.GetSettingByKey<string>(settingKey);
                if (!String.IsNullOrEmpty(settingValue) && !String.IsNullOrWhiteSpace(settingValue))
                {
                    var settings = JsonConvert.DeserializeObject<WB_SimpleMenuSettingsModel>(settingValue);
                    if (settings != null)
                    {
                        x.Settings = settings;
                    }
                }

                if (x.Items != null)
                {
                    x.Items.ForEach(i =>
                    {
                        var item = _simpleMenuItemRepo.GetById(i.Id);
                        i.IconUrlImage = item.IconUrlImage;
                        i.Url = item.Url;
                        i.Title = item.GetLocalized(il => il.Title, _workContext.WorkingLanguage.Id);
                    });
                }

            });
            //var model =
            //string settingKey = string.Format("wb_simplemenusettings.configs[{0}]", model.Id);
            //model.Settings= _settingService.GetSettingByKey<WB_SimpleMenuSettingsModel>(settingKey, new WB_SimpleMenuSettingsModel(), storeScope);

            return View("~/Plugins/Worldbuy.SimpleMenu/Views/PublicInfo.cshtml", model);
        }

        [NonAction]
        protected virtual void UpdateLocales(WB_SimpleMenuItem item, WB_SimpleMenuItemModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(item,
                                                               x => x.Title,
                                                               localized.Title,
                                                               localized.LanguageId);

            }
        }
    }
}
