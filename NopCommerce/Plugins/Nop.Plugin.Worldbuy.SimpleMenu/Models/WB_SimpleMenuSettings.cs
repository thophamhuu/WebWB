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

namespace Nop.Plugin.Worldbuy.SimpleMenu.Models
{
    public partial class WB_SimpleMenuSettingsModel
    {
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.UseIconImage")]
        public bool UseIconImage { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Worldbuy.SimpleMenu.IconToRight")]
        public bool IconToRight { get; set; }
    }
}
