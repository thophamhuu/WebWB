using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Worldbuy.SimpleMenu.Models
{
    public partial class WB_SimpleMenuModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.WidgetZone")]
        public string WidgetZone { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.IsActived")]
        public bool IsActived { get; set; }
        public virtual List<WB_SimpleMenuItemModel> Items { get; set; }
        public virtual List<SelectListItem> WidgetZones { get; set; }
        public virtual WB_SimpleMenuSettingsModel Settings { get; set; }
    }
}
