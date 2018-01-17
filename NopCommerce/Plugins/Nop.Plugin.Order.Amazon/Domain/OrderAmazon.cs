using Nop.Core;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Amazon.Domain
{
    public class OrderAmazon : BaseEntity, ILocalizedEntity
    {
        public int OrderId { get; set; }
        public string CartId { get; set; }
        public string HMAC { get; set; }
        public string PurchaseURL { get; set; }
        public DateTime CreateOrder { get; set; }
    }
}
