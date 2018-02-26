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
using Nop.Core.Infrastructure;
using Nop.Services.Media;

namespace Nop.Plugin.Worldbuy.AnyBanner.Services
{
    public class WB_AnyBannerItemService : IWB_AnyBannerItemService
    {
        private readonly IRepository<WB_AnyBanner> _AnyBannerRepo;
        private readonly IRepository<WB_AnyBannerItem> _AnyBannerItemRepo;
        private readonly ISettingService _settingService;
        public WB_AnyBannerItemService(
            ISettingService settingService,
            IRepository<WB_AnyBanner> AnyBannerRepo,
            IRepository<WB_AnyBannerItem> AnyBannerItemRepo)
        {
            this._settingService = settingService;

            this._AnyBannerItemRepo = AnyBannerItemRepo;
            this._AnyBannerRepo = AnyBannerRepo;
        }
        public IList<WB_AnyBannerItem> GetAllModelsByBannerId(int bannerId)
        {
            var result = (from smi in _AnyBannerItemRepo.Table
                          where smi.BannerID == bannerId
                          orderby smi.Order
                          select smi).ToList();
            return result;
        }

        public WB_AnyBannerItem GetModelById(int id)
        {
            var result = (from smi in _AnyBannerItemRepo.Table
                          where smi.Id == id
                          select smi).FirstOrDefault();
            return result;
        }
    }
}
