using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Plugin.Order.Amazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Amazon.Service
{
    public interface IOrderAmazonService
    {
        IPagedList<Nop.Core.Domain.Orders.Order> SearchOrders(int storeId = 0,
          int vendorId = 0, int customerId = 0,
          int productId = 0, int affiliateId = 0, int warehouseId = 0,
          int billingCountryId = 0, string paymentMethodSystemName = null,
          DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
          List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
          string billingEmail = null, string billingLastName = "",
          string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue);
        OrderAmazonModel GetOrderAmazonByOrderId(int orderId);
        CartCreateResponse CartCreate(OrderAmazonSettings amazonSettings, List<Item> items, string responseGroup = "Cart");
        CartGetResponse CartGet(OrderAmazonSettings amazonSettings, string cartId, string hmac, string responseGroup = "Cart");
        CartModifyResponse CartModify(OrderAmazonSettings amazonSettings, string cartId, string hmac, List<Item> items, string responseGroup = "Cart");
        CartAddResponse CartAdd(OrderAmazonSettings amazonSettings, string cartId, string hmac, List<Item> items, string responseGroup = "Cart");
        CartClearResponse CartClear(OrderAmazonSettings amazonSettings, string cartId, string hmac, string responseGroup = "Cart");
        //CartAmazonResponse CartAdd(string cartId,string HMAC,IList<CartItemAmazonRequest> items, string responseGroup = "");
    }
}
