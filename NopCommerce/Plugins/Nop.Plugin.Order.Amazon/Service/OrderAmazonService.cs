
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Plugin.Order.Amazon.Domain;
using Nop.Plugin.Order.Amazon.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nop.Core;
using Nop.Services.Orders;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Admin.Models.Orders;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Order.Amazon.Core;

namespace Nop.Plugin.Order.Amazon.Service
{
    public class OrderAmazonService : IOrderAmazonService
    {
        private readonly ISettingService _settingService;
        private readonly string responseCartCreate = "";

        private readonly IOrderService _orderService;

        private readonly IRepository<Nop.Core.Domain.Orders.Order> _orderRepository;
        private readonly IRepository<OrderAmazon> _orderAmazonRepository;
        //private readonly IRepository<OrderItemAmazon> _orderAmazonItemRepository;

        private readonly IRepository<ProductMapping> _productMappingRepository;
        private readonly IRepository<Product> _productRepository;
        public OrderAmazonService(ISettingService settingService,

            IOrderService orderService,

            IRepository<Nop.Core.Domain.Orders.Order> orderRepository,

            IRepository<ProductMapping> productMappingRepository,
            IRepository<Product> productRepository,
            IRepository<OrderAmazon> orderAmazonRepository
            //IRepository<OrderItemAmazon> orderAmazonItemRepository
            )
        {
            this._settingService = settingService;

            this._orderService = orderService;

            this._productMappingRepository = productMappingRepository;
            this._productRepository = productRepository;

            this._orderRepository = orderRepository;
            //this._orderAmazonItemRepository = orderAmazonItemRepository;
            this._orderAmazonRepository = orderAmazonRepository;
        }
        public CartCreateResponse CartCreate(OrderAmazonSettings amazonSettings, List<Item> items, string responseGroup = "Cart")
        {
            if (responseGroup == "")
                responseGroup = responseCartCreate;
            IDictionary<string, string> request = new Dictionary<string, string>();
            request["Operation"] = "CartCreate";
            int i = 1;
            foreach (var item in items)
            {
                if (!String.IsNullOrEmpty(item.OfferListingId) && !String.IsNullOrWhiteSpace(item.OfferListingId))
                {
                    request["Item." + i + ".OfferListingId"] = item.OfferListingId;
                }
                else
                {
                    request["Item." + i + ".ASIN"] = item.ASIN;
                }
                request["Item." + i + ".Quantity"] = item.Quantity.ToString();
                i++;
            }
            request["ResponseGroup"] = HttpUtility.UrlDecode(responseGroup);
            return Fetch<CartCreateResponse>(amazonSettings, request);
        }
        public CartGetResponse CartGet(OrderAmazonSettings amazonSettings, string cartId, string hmac, string responseGroup = "Cart")
        {
            if (responseGroup == "")
                responseGroup = responseCartCreate;
            IDictionary<string, string> request = new Dictionary<string, string>();
            request["Operation"] = "CartGet";
            request["ResponseGroup"] = HttpUtility.UrlDecode(responseGroup);

            request["CartId"] = cartId;
            request["HMAC"] = hmac;

            return Fetch<CartGetResponse>(amazonSettings, request);
        }



        public OrderAmazonModel GetOrderAmazonByOrderId(int orderId)
        {
            return _orderAmazonRepository.TableNoTracking.Select(x => new OrderAmazonModel
            {
                CartId = x.CartId,
                OrderId = x.OrderId,
                Id = x.Id,
                CreateOrder = x.CreateOrder,
                HMAC = x.HMAC,
                PurchaseURL = x.PurchaseURL
            }).FirstOrDefault(x => x.OrderId == orderId);
        }





        public CartModifyResponse CartModify(OrderAmazonSettings amazonSettings, string cartId, string hmac, List<Item> items, string responseGroup = "Cart")
        {
            if (responseGroup == "")
                responseGroup = responseCartCreate;
            IDictionary<string, string> request = new Dictionary<string, string>();
            request["Operation"] = "CartModify";

            request["CartId"] = cartId;
            request["HMAC"] = hmac;
            int i = 1;
            foreach (var item in items)
            {
                if (!String.IsNullOrEmpty(item.OfferListingId) && !String.IsNullOrWhiteSpace(item.OfferListingId))
                {
                    request["Item." + i + ".OfferListingId"] = item.OfferListingId;
                }
                else if (!String.IsNullOrEmpty(item.ASIN) && !String.IsNullOrWhiteSpace(item.ASIN))
                {
                    request["Item." + i + ".ASIN"] = item.ASIN;
                }
                else if (!String.IsNullOrEmpty(item.CartItemId) && !String.IsNullOrWhiteSpace(item.CartItemId))
                {
                    request["Item." + i + ".CartItemId"] = item.CartItemId;
                }
                request["Item." + i + ".Quantity"] = item.Quantity.ToString();
                i++;
            }
            request["ResponseGroup"] = HttpUtility.UrlDecode(responseGroup);

            return Fetch<CartModifyResponse>(amazonSettings, request);
        }

        public CartAddResponse CartAdd(OrderAmazonSettings amazonSettings, string cartId, string hmac, List<Item> items, string responseGroup = "Cart")
        {
            if (responseGroup == "")
                responseGroup = responseCartCreate;
            IDictionary<string, string> request = new Dictionary<string, string>();
            request["Operation"] = "CartAdd";
            request["CartId"] = cartId;
            request["HMAC"] = hmac;
            int i = 1;
            foreach (var item in items)
            {
                if (!String.IsNullOrEmpty(item.OfferListingId) && !String.IsNullOrWhiteSpace(item.OfferListingId))
                {
                    request["Item." + i + ".OfferListingId"] = item.OfferListingId;
                }
                else
                {
                    request["Item." + i + ".ASIN"] = item.ASIN;
                }
                request["Item." + i + ".Quantity"] = item.Quantity.ToString();
                i++;
            }
            request["ResponseGroup"] = HttpUtility.UrlDecode(responseGroup);

            return Fetch<CartAddResponse>(amazonSettings, request);
        }

        public CartClearResponse CartClear(OrderAmazonSettings amazonSettings, string cartId, string hmac, string responseGroup = "Cart")
        {
            if (responseGroup == "")
                responseGroup = responseCartCreate;
            IDictionary<string, string> request = new Dictionary<string, string>();

            request["Operation"] = "CartClear";
            request["CartId"] = cartId;
            request["HMAC"] = hmac;
            request["ResponseGroup"] = HttpUtility.UrlDecode(responseGroup);

            return Fetch<CartClearResponse>(amazonSettings, request);
        }

        public virtual IPagedList<Nop.Core.Domain.Orders.Order> SearchOrders(int storeId = 0,
            int vendorId = 0, int customerId = 0,
            int productId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            string billingEmail = null, string billingLastName = "",
            string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var pids = _productRepository.TableNoTracking.Where(x => x.Published && !x.Deleted).Select(x => x.Id).ToArray() ?? new int[0];
             pids= _productMappingRepository.TableNoTracking.Where(x => x.SourceId == 2 && pids.Contains(x.ProductId)).Select(x => x.ProductId).ToArray() ?? new int[0];
            var query = _orderRepository.Table.Where(x => x.OrderItems.Any(o => pids.Contains(o.ProductId)));
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.VendorId == vendorId));
            }
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);
            if (productId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.Id == productId));
            }
            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem =>
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (orderItem.Product.ManageInventoryMethodId == manageStockInventoryMethodId &&
                        orderItem.Product.UseMultipleWarehouses &&
                        orderItem.Product.ProductWarehouseInventory.Any(pwi => pwi.WarehouseId == warehouseId))
                        ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((orderItem.Product.ManageInventoryMethodId != manageStockInventoryMethodId ||
                        !orderItem.Product.UseMultipleWarehouses) &&
                        orderItem.Product.WarehouseId == warehouseId))
                        );
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (!String.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);
            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            if (!String.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            if (!String.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !String.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!String.IsNullOrEmpty(orderNotes))
                query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderNotes)));
            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return new PagedList<Nop.Core.Domain.Orders.Order>(query, pageIndex, pageSize);
        }

        private T Fetch<T>(OrderAmazonSettings amazonSettings, IDictionary<string, string> request) where T : class
        {
            request["Service"] = amazonSettings.Service;
            request["AssociateTag"] = amazonSettings.AssociateTag;
            request["Version"] = amazonSettings.Version;
            SignedRequestHelper helper = new SignedRequestHelper(amazonSettings.AWSAccessKeyID, amazonSettings.AWSSecretKey, amazonSettings.Endpoint);
            string requestUrl = helper.Sign(request);
            string _awsNamespace = string.Format("http://webservices.amazon.com/{0}/{1}", "AWSECommerceService", "2013-08-01");
            try
            {
                HttpWebRequest webRequest = (System.Net.HttpWebRequest)HttpWebRequest.Create(requestUrl);
                webRequest.UserAgent = "Chrome/56.0.2924.87";
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 20000;
                using (WebResponse response = webRequest.GetResponseAsync().Result)
                {
                    using (StreamReader xmlReader = new StreamReader(response.GetResponseStream()))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T), _awsNamespace);
                        T result = (T)serializer.Deserialize(xmlReader);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(ex.Message, ex);
            }
            return null;
        }

    }
}
