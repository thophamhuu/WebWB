using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Worldbuy.AnyBanner.Models;
using Nop.Core.Data;
using Nop.Plugin.Worldbuy.AnyBanner.Domain;
using Nop.Services.Configuration;
using System.Web.Mvc;
using System.Xml;
using System.Web;

namespace Nop.Plugin.Worldbuy.AnyBanner.Services
{
    public class WB_AnyBannerService : IWB_AnyBannerService
    {
        private readonly IRepository<WB_AnyBanner> _AnyBannerRepo;
        private readonly IRepository<WB_AnyBannerItem> _AnyBannerItemRepo;
        private readonly ISettingService _settingService;
        public WB_AnyBannerService(
            ISettingService settingService,
            IRepository<WB_AnyBanner> AnyBannerRepo,
            IRepository<WB_AnyBannerItem> AnyBannerItemRepo)
        {
            this._settingService = settingService;

            this._AnyBannerItemRepo = AnyBannerItemRepo;
            this._AnyBannerRepo = AnyBannerRepo;
        }
        public IList<WB_AnyBannerModel> GetAllModels(string widgetZone = "", bool? isActived = null)
        {
            var query = (from sm in _AnyBannerRepo.TableNoTracking
                         select sm);
            if (widgetZone!="")
            {
                query = query.Where(sm => sm.WidgetZone.ToLower() == widgetZone.ToLower());
            }
            if (isActived.HasValue)
            {
                query = query.Where(sm => sm.IsActived == isActived.Value);
            }
            var result = query.Select(sm => new WB_AnyBannerModel
            {
                Id = sm.Id,
                Name = sm.Name,
                WidgetZone = sm.WidgetZone,
                IsActived=sm.IsActived
            }).ToList();
            if (result != null)
            {
                result.ForEach(x =>
                {
                    var queryItems = (from smi in _AnyBannerItemRepo.TableNoTracking
                                      where smi.BannerID == x.Id
                                      select smi);
                    if (isActived.HasValue)
                    {
                        queryItems = queryItems.Where(smi => smi.IsActived == isActived.Value);
                    }
                    x.Items = queryItems.OrderBy(smi => smi.Order).Select(smi => new WB_AnyBannerItemModel
                    {
                        Id = smi.Id,
                        BannerID = smi.BannerID,
                        Title = smi.Title,
                        Url = smi.Url,
                        Alt = smi.Alt,
                        IsActived = smi.IsActived,
                        ImageUrl = smi.ImageUrl,
                        Order = smi.Order
                    }).ToList();
                });
            }
            return result;
        }

        public WB_AnyBannerModel GetModelById(int id)
        {
            var result = (from sm in _AnyBannerRepo.TableNoTracking
                          where sm.Id == id
                          select new WB_AnyBannerModel
                          {
                              Id = sm.Id,
                              Name = sm.Name,
                              WidgetZone = sm.WidgetZone,
                              IsActived = sm.IsActived
                          }).FirstOrDefault();
            if (result != null)
            {
                result.Items = (from smi in _AnyBannerItemRepo.TableNoTracking
                                where smi.BannerID == result.Id
                                orderby smi.Order
                                select new WB_AnyBannerItemModel
                                {
                                    Id = smi.Id,
                                    BannerID = smi.BannerID,
                                    Title = smi.Title,
                                    Url = smi.Url,
                                    IsActived = smi.IsActived,
                                    ImageUrl = smi.ImageUrl,
                                }).ToList();
            }
            return result;
        }

        public IList<SelectListItem> GetWidgetZones()
        {
            var widgetZones = new List<SelectListItem>();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(HttpContext.Current.Server.MapPath("~/Plugins/Worldbuy.AnyBanner/SupportedWidgetZones.xml"));
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
