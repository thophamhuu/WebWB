using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Order.Ebay.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Services
{
    public interface IOrderEbayService
    {
        void DeleteOrderEbay(OrderEbay orderEbay);
        void InsertOrderEbay(OrderEbay orderEbay);
        void UpdateOrderEbay(OrderEbay orderEbay);
        PagedList<OrderEbay> GetAllOrderEbay(int pageIndex = 0, int pageSize = int.MaxValue);
        OrderEbay GetOrderEbay(int id);
        void DeleteOrderEbayDetail(OrderEbayDetail orderEbayDetail);
        void InsertOrderEbayDetail(OrderEbayDetail orderEbayDetail);
        void UpdateOrderEbayDetail(OrderEbayDetail orderEbayDetail);
        IList<OrderEbayDetail> GetAllOrderEbayDetail(int orderEbayId);
        OrderEbayDetail GetOrderEbayDetail(int id);
        bool CheckProductEbay(Product product);
        string GetProductSource(Product product);
    }
}
