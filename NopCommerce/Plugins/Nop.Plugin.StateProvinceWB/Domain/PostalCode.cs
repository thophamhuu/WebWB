using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Worldbuy.StateProvinceWB.Domain
{
    public partial class StateProvincePostalCode : BaseEntity, ILocalizedEntity
    {
        public int StateProvinceID { get; set; }
        public string PostalCode { get; set; }
    }
}
