using Nop.Plugin.Worldbuy.SimpleMenu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Worldbuy.SimpleMenu.Services
{
    public interface IWB_SimpleMenuService
    {
        WB_SimpleMenuModel GetModelById(int id);
        IList<WB_SimpleMenuModel> GetAllModels(string widgetZone="",bool? isActived = null);

        IList<SelectListItem> GetWidgetZones();
    }
}
