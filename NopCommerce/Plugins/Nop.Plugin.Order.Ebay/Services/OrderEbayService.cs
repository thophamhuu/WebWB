using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Order.Ebay.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Order.Ebay.Services
{
    public partial class OrderEbayService : IOrderEbayService
    {
        #region Fields

        private readonly IRepository<OrderEbay> _orderEbayRepository;
        private readonly IRepository<OrderEbayDetail> _orderEbayDetailRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<ProductMapping> _productMappingRepository;

        #endregion

        #region Ctor

        public OrderEbayService(IRepository<OrderEbay> orderEbayRepository, ICacheManager cacheManager, IEventPublisher eventPublisher, IRepository<OrderEbayDetail> orderEbayDetailRepository,
            IRepository<ProductMapping> productMappingRepository)
        {
            this._orderEbayRepository = orderEbayRepository;
            this._orderEbayDetailRepository = orderEbayDetailRepository;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._productMappingRepository = productMappingRepository;
        }

        #endregion

        #region Methods

        public virtual void DeleteOrderEbay(OrderEbay orderEbay)
        {
            if (orderEbay == null)
                throw new ArgumentNullException("orderEbay");

            _orderEbayRepository.Delete(orderEbay);
        }

        public virtual void InsertOrderEbay(OrderEbay orderEbay)
        {
            if (orderEbay == null)
                throw new ArgumentNullException("orderEbay");

            _orderEbayRepository.Insert(orderEbay);
        }

        public virtual void UpdateOrderEbay(OrderEbay orderEbay)
        {
            if (orderEbay == null)
                throw new ArgumentNullException("orderEbay");

            _orderEbayRepository.Update(orderEbay);
        }


        public virtual PagedList<OrderEbay> GetAllOrderEbay(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _orderEbayRepository.Table;

            query = query.OrderByDescending(o => o.Id);

            //database layer paging
            return new PagedList<OrderEbay>(query, pageIndex, pageSize);
        }

        public OrderEbay GetOrderEbay(int id)
        {
            return _orderEbayRepository.Table.FirstOrDefault(m => m.Id == id);
        }

        public virtual void DeleteOrderEbayDetail(OrderEbayDetail orderEbayDetail)
        {
            if (orderEbayDetail == null)
                throw new ArgumentNullException("orderEbayDetail");

            _orderEbayDetailRepository.Delete(orderEbayDetail);
        }

        public virtual void InsertOrderEbayDetail(OrderEbayDetail orderEbayDetail)
        {
            if (orderEbayDetail == null)
                throw new ArgumentNullException("orderEbayDetail");

            _orderEbayDetailRepository.Insert(orderEbayDetail);
        }

        public virtual void UpdateOrderEbayDetail(OrderEbayDetail orderEbayDetail)
        {
            if (orderEbayDetail == null)
                throw new ArgumentNullException("orderEbayDetail");

            _orderEbayDetailRepository.Update(orderEbayDetail);
        }


        public virtual IList<OrderEbayDetail> GetAllOrderEbayDetail(int orderEbayId)
        {
            var query = from gp in _orderEbayDetailRepository.Table
                        where gp.OrderId == orderEbayId
                        orderby gp.Id
                        select gp;
            var records = query.ToList();
            return records;
        }

        public OrderEbayDetail GetOrderEbayDetail(int id)
        {
            return _orderEbayDetailRepository.Table.FirstOrDefault(m => m.Id == id);
        }

        public bool CheckProductEbay(Product product)
        {
            var query = from gp in _productMappingRepository.Table
                        where gp.ProductId == product.Id && gp.SourceId == 1
                        orderby gp.Id
                        select gp;

            if (query.FirstOrDefault() == null)
                return false;
            else
                return true;
        }

        public string GetProductSource(Product product)
        {
            var query = from gp in _productMappingRepository.Table
                        where gp.ProductId == product.Id && gp.SourceId == 1
                        orderby gp.Id
                        select gp;
            return query.FirstOrDefault().ProductSourceId;
        }

        #endregion
    }
}
