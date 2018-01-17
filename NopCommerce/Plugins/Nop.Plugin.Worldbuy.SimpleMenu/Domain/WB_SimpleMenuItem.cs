using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Worldbuy.SimpleMenu.Domain
{
    public partial class WB_SimpleMenuItem : BaseEntity, ILocalizedEntity
    {
        public int MenuID { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string IconUrlImage { get; set; }
        public int Order { get; set; }
    }
}
