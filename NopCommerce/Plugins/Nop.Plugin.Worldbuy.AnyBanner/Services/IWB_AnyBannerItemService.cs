using Nop.Plugin.Worldbuy.AnyBanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Worldbuy.AnyBanner.Services
{
    public interface IWB_AnyBannerItemService
    {
        WB_AnyBannerItemModel GetModelById(int id);
        IList<WB_AnyBannerItemModel> GetAllModelsByBannerId(int bannerId);
    }
}
