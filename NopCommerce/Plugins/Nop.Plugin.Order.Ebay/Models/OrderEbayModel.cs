using Nop.Plugin.Order.Ebay.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Models
{
    public class OrderEbayModel
    {
        public OrderEbay OrderEbay { get; set; }
        public IList<OrderEbayDetail> OrderEbayDetail { get; set; }
    }
}
