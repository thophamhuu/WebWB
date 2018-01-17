using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Worldbuy.AnyBanner.Domain
{
    public partial class WB_AnyBannerItem : BaseEntity
    {
        public int BannerID { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActived { get; set; }
        public int Order { get; set; }
    }
}
