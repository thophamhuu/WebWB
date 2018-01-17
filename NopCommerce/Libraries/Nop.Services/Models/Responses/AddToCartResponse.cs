using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Api.Models.Responses
{
    public class AddToCartResponse
    {
        public IList<string> warnings { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems{ get; set; }
    }
}