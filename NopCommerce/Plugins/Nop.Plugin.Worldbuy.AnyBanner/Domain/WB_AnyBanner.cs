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
    public partial class WB_AnyBanner : BaseEntity
    {
        public string Name { get; set; }
        public string WidgetZone { get; set; }
        public bool IsActived { get; set; }
    }
}
