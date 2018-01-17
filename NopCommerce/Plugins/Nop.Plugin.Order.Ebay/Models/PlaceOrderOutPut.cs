using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Models
{
    public class PlaceOrderOutPut
    {
        public string purchaseOrderHref { get; set; }
        public string purchaseOrderId { get; set; }
        public string purchaseOrderPaymentStatus { get; set; }
        public List<Warning> warnings { get; set; }
    }

}
