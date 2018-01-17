using Nop.Plugin.Worldbuy.AnyBanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Worldbuy.AnyBanner.Services
{
    public interface IWB_AnyBannerService
    {
        WB_AnyBannerModel GetModelById(int id);
        IList<WB_AnyBannerModel> GetAllModels(string widgetZone = "",bool? isActived=null);

        IList<SelectListItem> GetWidgetZones();
    }
}
