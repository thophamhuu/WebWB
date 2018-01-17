using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Plugin.Worldbuy.Models;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Worldbuy.AnyBanner.Models
{
    public partial class WB_AnyBannerModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.WidgetZone")]
        public string WidgetZone { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.IsActived")]
        public bool IsActived { get; set; }
        public virtual List<WB_AnyBannerItemModel> Items { get; set; }
        public virtual List<SelectListItem> WidgetZones { get; set; }
        public virtual WB_ColumnOnRowModel Settings { get; set; }
       
    }
}
