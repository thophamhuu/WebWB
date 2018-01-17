using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Worldbuy.SimpleMenu.Models
{
    public partial class WB_SimpleMenuItemModel : BaseNopEntityModel, ILocalizedModel<WB_SimpleMenuItemLocalizedModel>
    {
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.Item.Menu")]
        public int MenuID { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.Item.Title")]
        public string Title { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.Item.Order")]
        public int Order { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.Item.Url")]
        public string Url { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.Item.IconUrlImage")]
        public string IconUrlImage { get; set; }
        public IList<WB_SimpleMenuItemLocalizedModel> Locales { get; set; }
    }

    public partial class WB_SimpleMenuItemLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.Item.Title")]
        public string Title { get; set; }
    }

}
