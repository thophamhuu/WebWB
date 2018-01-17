using Nop.Plugin.Worldbuy.SimpleMenu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Worldbuy.SimpleMenu.Services
{
    public interface IWB_SimpleMenuItemService
    {
        WB_SimpleMenuItemModel GetModelById(int id);
        IList<WB_SimpleMenuItemModel> GetAllModelsByMenuId(int menuId);
    }
}
