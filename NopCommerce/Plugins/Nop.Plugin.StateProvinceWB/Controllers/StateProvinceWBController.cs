using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Worldbuy.StateProvinceWB.Domain;
using Nop.Plugin.Worldbuy.StateProvinceWB.Models;
using Nop.Plugin.Worldbuy.StateProvinceWB.Services;
using Nop.Services.Configuration;
using Nop.Services.Directory;
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

namespace Nop.Plugin.Worldbuy.StateProvinceWB.Controllers
{
    [AdminAuthorize]
    public class StateProvinceWBController : BasePluginController
    {
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStateProvinceWBService _stateProvinceWBService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IStateProvinceWBImportManager _importManager;
        private readonly ILocalizationService _localizationService;

        public StateProvinceWBController(ICountryService countryService,
           IStateProvinceService stateProvinceService,
           IStateProvinceWBService stateProvinceWBService,
            IWorkContext workContext,
           IStoreContext storeContext,
           IStoreService storeService,
           ISettingService settingService,
           ILocalizationService localizationService,
           IStateProvinceWBImportManager importManager,
           IPluginFinder _pluginFinder)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._stateProvinceWBService = stateProvinceWBService;
            this._importManager = importManager;
        }
        public ActionResult List(int countryId = 1)
        {
            ViewBag.Country = _countryService.GetAllCountries().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == countryId
            }).ToList();
            ViewBag.CountryID = countryId;
            return View("~/Plugins/StateProvince.WB/Views/List.cshtml");
        }

        public JsonResult Read(int countryId = 1)
        {
            var model = _stateProvinceWBService.GetAllStateProvinceWBByCountryId(countryId).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ImportFromXlsx(int countryId)
        {
            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportProvincesFromXlsx(file.InputStream, countryId);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.StateProvinceWB.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }

        }

        [HttpPost]
        public ActionResult Update()
        {
            bool status = false;
            string message = "";
            if (Request.Form.Count > 0)
            {
                var models = JsonConvert.DeserializeObject<List<StateProvinceWBModel>>(Request.Form[0]);

                if (models != null)
                {
                    var model = models[0];
                    var stateProvince = _stateProvinceService.GetStateProvinceById(model.Id);
                    if (stateProvince != null && stateProvince != null)
                    {
                        stateProvince.Name = model.Name;
                        stateProvince.Abbreviation = model.Abbreviation;


                        _stateProvinceService.UpdateStateProvince(stateProvince);

                        var stateProvinceWBs = _stateProvinceWBService.GetStateProvinceWBsByStateProvinceId(stateProvince.Id);
                        var postalCodes = new List<String>();
                        if (!String.IsNullOrEmpty(model.PostalCode))
                            postalCodes = model.PostalCode.Split(',').ToList();
                        if (stateProvinceWBs != null)
                        {
                            var nonList = stateProvinceWBs.Where(x => !postalCodes.Contains(x.PostalCode)).ToList();
                            if (nonList != null)
                            {
                                _stateProvinceWBService.Delete(nonList);
                            }
                            foreach(var postalCode in postalCodes)
                            {
                                if (_stateProvinceWBService.GetByPostalCodeAndProvinceID(postalCode, stateProvince.Id) == null)
                                {
                                    var stateProvinceWB = new StateProvincePostalCode
                                    {
                                        Id = 0,
                                        PostalCode = postalCode,
                                        StateProvinceID = stateProvince.Id
                                    };
                                    _stateProvinceWBService.Insert(stateProvinceWB);
                                }
                                
                            }
                            
                        }
                    }
                    model = _stateProvinceWBService.GetStateProvinceWBModelByStateProvinceId(model.Id);
                    return Json(model);
                }
            }
            return Json(new { Status = status, Message = message });
        }

        [HttpPost]
        public ActionResult Delete()
        {
            bool status = false;
            string message = "";
            if (Request.Form.Count > 0)
            {
                var models = JsonConvert.DeserializeObject<List<StateProvinceWBModel>>(Request.Form[0]);

                if (models != null)
                {
                    var model = models[0];
                    var stateProvince = _stateProvinceService.GetStateProvinceById(model.Id);
                    if (stateProvince != null && stateProvince != null)
                    {
                        var stateProvinceWB = _stateProvinceWBService.GetStateProvinceWBsByStateProvinceId(stateProvince.Id);
                        if (stateProvinceWB != null)
                        {
                            _stateProvinceWBService.Delete(stateProvinceWB);
                        }
                        _stateProvinceService.DeleteStateProvince(stateProvince);
                    }
                }
            }
            return Json(new { Status = status, Message = message });
        }
    }
}
