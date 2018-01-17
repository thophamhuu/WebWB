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

namespace Nop.Plugin.Worldbuy.AnyBanner.Models
{
    public partial class WB_AnyBannerItemModel : BaseNopEntityModel    {
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Banner")]
        public int BannerID { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Title")]
        public string Title { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Alt")]
        public string Alt { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Url")]
        public string Url { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.ImageUrl")]
        public string ImageUrl { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.IsActived")]
        public bool IsActived { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.AnyBanner.Item.Order")]
        public int Order { get; set; }
    }

}
