using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Worldbuy.SimpleMenu.Models;
using Nop.Core.Data;
using Nop.Plugin.Worldbuy.SimpleMenu.Domain;
using Nop.Services.Configuration;
using System.Web.Mvc;
using System.Xml;
using System.Web;

namespace Nop.Plugin.Worldbuy.SimpleMenu.Services
{
    public class WB_SimpleMenuService : IWB_SimpleMenuService
    {
        private readonly IRepository<WB_SimpleMenu> _simpleMenuRepo;
        private readonly IRepository<WB_SimpleMenuItem> _simpleMenuItemRepo;
        private readonly ISettingService _settingService;
        public WB_SimpleMenuService(
            ISettingService settingService,
            IRepository<WB_SimpleMenu> simpleMenuRepo,
            IRepository<WB_SimpleMenuItem> simpleMenuItemRepo)
        {
            this._settingService = settingService;

            this._simpleMenuItemRepo = simpleMenuItemRepo;
            this._simpleMenuRepo = simpleMenuRepo;
        }
        public IList<WB_SimpleMenuModel> GetAllModels(string widgetZone = "", bool? isActived = null)
        {
            var query = (from sm in _simpleMenuRepo.TableNoTracking
                         select sm);
            if (widgetZone != "")
            {
                query = query.Where(sm => sm.WidgetZone == widgetZone);
            }
            if (isActived.HasValue)
            {
                query = query.Where(sm => sm.IsActived == isActived.Value);
            }
            var result = query.Select(sm => new WB_SimpleMenuModel
            {
                Id = sm.Id,
                Name = sm.Name,
                WidgetZone = sm.WidgetZone,
                IsActived = sm.IsActived
            }).ToList();
            if (result != null)
            {
                result.ForEach(x =>
                {
                    x.Items = (from smi in _simpleMenuItemRepo.TableNoTracking
                               where smi.MenuID == x.Id
                               orderby smi.Order
                               select new WB_SimpleMenuItemModel
                               {
                                   Id = smi.Id,
                                   MenuID = smi.MenuID,
                                   Title = smi.Title,
                                   Url = smi.Url,
                                   Order = smi.Order
                               }).ToList();
                });
            }
            return result;
        }

        public WB_SimpleMenuModel GetModelById(int id)
        {
            var result = (from sm in _simpleMenuRepo.TableNoTracking
                          where sm.Id == id
                          select new WB_SimpleMenuModel
                          {
                              Id = sm.Id,
                              Name = sm.Name,
                              WidgetZone = sm.WidgetZone,
                              IsActived=sm.IsActived,
                          }).FirstOrDefault();
            if (result != null)
            {
                result.Items = (from smi in _simpleMenuItemRepo.TableNoTracking
                                where smi.MenuID == result.Id
                                orderby smi.Order
                                select new WB_SimpleMenuItemModel
                                {
                                    Id = smi.Id,
                                    MenuID = smi.MenuID,
                                    Title = smi.Title,
                                    Url = smi.Url,
                                    Order = smi.Order
                                }).ToList();
            }
            return result;
        }

        public IList<SelectListItem> GetWidgetZones()
        {
            var widgetZones = new List<SelectListItem>();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(HttpContext.Current.Server.MapPath("~/Plugins/Worldbuy.SimpleMenu/SupportedWidgetZones.xml"));
            XmlNode SupportedWidgetZones = xmlDoc.SelectSingleNode("SupportedWidgetZones");
            if (SupportedWidgetZones != null)
            {
                var WidgetZones = SupportedWidgetZones.SelectNodes("WidgetZone");
                if (WidgetZones != null)
                {
                    foreach (XmlNode node in WidgetZones)
                    {
                        var value = node.Attributes["code"] != null ? node.Attributes["code"].Value : "";
                        var text = node.InnerText;
                        if (!String.IsNullOrEmpty(value) && !String.IsNullOrWhiteSpace(value))
                            widgetZones.Add(new SelectListItem { Value = value, Text = text });
                    }
                }
            }
            return widgetZones;
        }
    }
}
