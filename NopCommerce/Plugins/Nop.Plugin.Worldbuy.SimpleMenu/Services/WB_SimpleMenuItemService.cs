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
    public class WB_SimpleMenuItemService : IWB_SimpleMenuItemService
    {
        private readonly IRepository<WB_SimpleMenu> _simpleMenuRepo;
        private readonly IRepository<WB_SimpleMenuItem> _simpleMenuItemRepo;
        private readonly ISettingService _settingService;
        public WB_SimpleMenuItemService(
            ISettingService settingService,
            IRepository<WB_SimpleMenu> simpleMenuRepo,
            IRepository<WB_SimpleMenuItem> simpleMenuItemRepo)
        {
            this._settingService = settingService;

            this._simpleMenuItemRepo = simpleMenuItemRepo;
            this._simpleMenuRepo = simpleMenuRepo;
        }
        public IList<WB_SimpleMenuItemModel> GetAllModelsByMenuId(int menuId)
        {
            var result = (from smi in _simpleMenuItemRepo.Table
                          where smi.MenuID == menuId
                          orderby smi.Order
                          select new WB_SimpleMenuItemModel
                          {
                              Id = smi.Id,
                              Title = smi.Title,
                              Url = smi.Url,
                              IconUrlImage = smi.IconUrlImage,
                              MenuID = smi.MenuID,
                              Order = smi.Order
                          }).ToList();
            return result;
        }

        public WB_SimpleMenuItemModel GetModelById(int id)
        {
            var result = (from smi in _simpleMenuItemRepo.Table
                          where smi.Id == id
                          select new WB_SimpleMenuItemModel
                          {
                              Id = smi.Id,
                              Title = smi.Title,
                              Url = smi.Url,
                              Order = smi.Order,
                              MenuID = smi.MenuID,
                              IconUrlImage = smi.IconUrlImage
                          }).FirstOrDefault();

            return result;
        }
    }
}
