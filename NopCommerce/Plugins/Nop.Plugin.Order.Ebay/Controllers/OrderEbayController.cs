using Newtonsoft.Json;
using Nop.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Affiliate.CategoryMap.Services;
using Nop.Plugin.Affiliate.Ebay.Models;
using Nop.Plugin.Affiliate.Ebay.Services;
using Nop.Plugin.Order.Ebay.Domain;
using Nop.Plugin.Order.Ebay.Models;
using Nop.Plugin.Order.Ebay.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Order.Ebay.Controllers
{
    public partial class OrderEbayController : BasePluginController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly ICacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPictureService _pictureService;
        private readonly IProductMappingService _productMappingService;
        private readonly IOrderEbayService _orderEbayService;
        private readonly IRepository<ProductMapping> _productMappingRepository;

        #endregion

        #region Constructors

        public OrderEbayController(IWorkContext workContext, IStoreContext storeContext, IStoreService storeService, ISettingService settingService, ILocalizationService localizationService,
            ICategoryService categoryService, IProductService productService, ICacheManager cacheManager, IDateTimeHelper dateTimeHelper, IOrderService orderService,
            IPriceFormatter priceFormatter, IPictureService pictureService, IProductMappingService productMappingService, IOrderEbayService orderEbayService, IRepository<ProductMapping> productMappingRepository)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._workContext = workContext;
            this._categoryService = categoryService;
            this._productService = productService;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
            this._orderService = orderService;
            this._priceFormatter = priceFormatter;
            this._pictureService = pictureService;
            this._productMappingService = productMappingService;
            this._orderEbayService = orderEbayService;
            this._productMappingRepository = productMappingRepository;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void SaveCategoryMappings(Product product, List<int> SelectedCategoryIds)
        {
            var existingProductCategories = _categoryService.GetProductCategoriesByProductId(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    _categoryService.DeleteProductCategory(existingProductCategory);

            //add categories
            foreach (var categoryId in SelectedCategoryIds)
                if (existingProductCategories.FindProductCategory(product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = _categoryService.GetProductCategoriesByCategoryId(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    _categoryService.InsertProductCategory(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
        }

        #endregion

        #region Methods

        public ActionResult ListOrder()
        {
            var model = new OrderListModel();

            return View("~/Plugins/Order.Ebay/Views/ListOrder.cshtml", model);
        }

        [HttpPost]
        public ActionResult ListOrder(DataSourceRequest command, OrderListModel model)
        {
            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var orderStatusIds = new List<int>() { (int)OrderStatus.Pending };

            //load orders
            var orders = _orderService.SearchOrders(createdFromUtc: startDateValue, createdToUtc: endDateValue, osIds: orderStatusIds, pageIndex: command.Page - 1, pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new OrderModel
                    {
                        Id = x.Id,
                        StoreName = store != null ? store.Name : "Unknown",
                        OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                        OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        OrderStatusId = x.OrderStatusId,
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatusId = x.PaymentStatusId,
                        ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatusId = x.ShippingStatusId,
                        CustomerEmail = x.BillingAddress.Email,
                        CustomerFullName = string.Format("{0} {1}", x.BillingAddress.FirstName, x.BillingAddress.LastName),
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        CustomOrderNumber = x.CustomOrderNumber
                    };
                }),
                Total = orders.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult ListOrderEbay()
        {
            return View("~/Plugins/Order.Ebay/Views/ListOrderEbay.cshtml");
        }

        [HttpPost]
        public ActionResult ListOrderEbay(DataSourceRequest command)
        {
            var orders = _orderEbayService.GetAllOrderEbay(pageIndex: command.Page - 1, pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = orders.AsEnumerable(),

                Total = orders.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public async Task<ActionResult> OrderEbay(int id)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ebaySettings = _settingService.LoadSetting<ConfigurationModel>(storeScope);

            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted)
                return RedirectToAction("ListOrder");

            var listItem = order.OrderItems;
            var listProduct = new List<Product>();

            foreach (var item in listItem)
            {
                var product = _orderEbayService.CheckProductEbay(item.Product);
                if (product)
                    listProduct.Add(item.Product);
            }

            if (listProduct != null)
            {
                // initiate
                var clientapi = new HttpClient();
                clientapi.BaseAddress = new Uri("https://api.sandbox.ebay.com/");
                clientapi.DefaultRequestHeaders.Clear();
                clientapi.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientapi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ebaySettings.Token);

                var input = new InitiateInput();
                input.contactEmail = order.ShippingAddress.Email == null ? order.Customer.Email : order.ShippingAddress.Email;
                input.contactFirstName = order.ShippingAddress.FirstName;
                input.contactLastName = order.ShippingAddress.LastName;

                var shippingAddress = new ShippingAddress();
                shippingAddress.recipient = order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName;
                shippingAddress.phoneNumber = "617 555 1212";
                shippingAddress.addressLine1 = "No. 37, Jalan Gr 1/4, Cassia Garden Residence";
                shippingAddress.city = "Cyberjaya";
                shippingAddress.stateOrProvince = "Selangor";
                shippingAddress.postalCode = "62000";
                shippingAddress.country = "MY";

                var creditCard = new CreditCard();
                creditCard.accountHolderName = "Frank Smith";
                creditCard.cardNumber = "5100000001598174";
                creditCard.cvvNumber = "012";
                creditCard.expireMonth = 10;
                creditCard.expireYear = 2019;
                creditCard.brand = "MASTERCARD";

                var billingAddress = new BillingAddress();
                billingAddress.firstName = "Frank";
                billingAddress.lastName = "Smith";
                billingAddress.addressLine1 = "3737 Any St";
                billingAddress.city = "San Jose";
                billingAddress.stateOrProvince = "CA";
                billingAddress.postalCode = "95134";
                billingAddress.country = "US";
                creditCard.billingAddress = billingAddress;

                var lineItemInputs = new List<LineItemInput>();
                foreach (var item in listProduct)
                {
                    var lineItem = new LineItemInput();
                    lineItem.itemId = _orderEbayService.GetProductSource(item);
                    lineItem.quantity = order.OrderItems.Where(c => c.ProductId == item.Id).FirstOrDefault().Quantity;

                    lineItemInputs.Add(lineItem);
                }

                input.creditCard = creditCard;
                input.shippingAddress = shippingAddress;
                input.lineItemInputs = lineItemInputs;

                var json = (object)input;
                HttpResponseMessage Res = await clientapi.PostAsJsonAsync("buy/order/v1/guest_checkout_session/initiate", json);

                if (Res.IsSuccessStatusCode)
                {
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<InitiateOutput>(EmpResponse);

                    //place order
                    var clientapiplaceorder = new HttpClient();
                    clientapiplaceorder.BaseAddress = new Uri("https://api.sandbox.ebay.com/");
                    clientapiplaceorder.DefaultRequestHeaders.Clear();
                    clientapiplaceorder.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    clientapiplaceorder.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ebaySettings.Token);

                    HttpResponseMessage Respon = await clientapiplaceorder.PostAsync("buy/order/v1/guest_checkout_session/" + result.checkoutSessionId + "/place_order", null);

                    if (Respon.IsSuccessStatusCode)
                    {
                        var EmpResponsse = Res.Content.ReadAsStringAsync().Result;
                        var resulst = JsonConvert.DeserializeObject<PlaceOrderOutPut>(EmpResponse);

                        //getPurchaseOrder
                        var clientapiGet = new HttpClient();
                        clientapiGet.BaseAddress = new Uri("https://api.sandbox.ebay.com/");
                        clientapiGet.DefaultRequestHeaders.Clear();
                        clientapiGet.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        clientapiGet.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ebaySettings.Token);

                        HttpResponseMessage Respon1 = await clientapiGet.GetAsync("buy/order/v1/guest_purchase_order/" + resulst.purchaseOrderId);
                        if (Respon.IsSuccessStatusCode)
                        {
                            var EmpResponses = Res.Content.ReadAsStringAsync().Result;
                            var result1 = JsonConvert.DeserializeObject<PurchaseOrderOutPut>(EmpResponse);

                            var ebayOder = new OrderEbay();
                            ebayOder.OrderId = result1.purchaseOrderId;
                            ebayOder.OrderStatus = result1.purchaseOrderStatus;
                            ebayOder.PaymentStatus = result1.purchaseOrderPaymentStatus;
                            //ebayOder.PricingSummary = result1.pricingSummary.total.value;
                            ebayOder.RefundedAmount = result1.refundedAmount.value;
                            ebayOder.CreationDate = result1.purchaseOrderCreationDate;

                            ebayOder.Adjustment = result1.pricingSummary.adjustment.amount.value;
                            ebayOder.DeliveryCost = result1.pricingSummary.deliveryCost.value;
                            ebayOder.DeliveryDiscount = result1.pricingSummary.deliveryDiscount.value;
                            ebayOder.Fee = result1.pricingSummary.fee.value;
                            ebayOder.PriceDiscount = result1.pricingSummary.priceDiscount.value;
                            ebayOder.PriceSubtotal = result1.pricingSummary.priceSubtotal.value;
                            ebayOder.Tax = result1.pricingSummary.tax.value;
                            ebayOder.Total = result1.pricingSummary.total.value;

                            _orderEbayService.InsertOrderEbay(ebayOder);

                            foreach(var item in result1.lineItems)
                            {
                                var orderdetai = new OrderEbayDetail();
                                orderdetai.OrderId = ebayOder.Id;
                                orderdetai.ItemId = item.itemId;
                                orderdetai.LineItemId = item.lineItemId;
                                orderdetai.LineItemPaymentStatus = item.lineItemPaymentStatus;
                                orderdetai.LineItemStatus = item.lineItemStatus;
                                orderdetai.MaxEstimatedDeliveryDate = item.shippingDetail.maxEstimatedDeliveryDate;
                                orderdetai.MinEstimatedDeliveryDate = item.shippingDetail.minEstimatedDeliveryDate;
                                orderdetai.ShippingCarrierCode = item.shippingDetail.shippingCarrierCode;
                                orderdetai.ShippingServiceCode = item.shippingDetail.shippingServiceCode;
                                orderdetai.NetPrice = item.netPrice.value;
                                orderdetai.Quantity = item.quantity;
                                orderdetai.FeedbackPercentage = item.seller.feedbackPercentage;
                                orderdetai.FeedbackScore = int.Parse(item.seller.feedbackScore);
                                orderdetai.SellerAccountType = item.seller.sellerAccountType;
                                orderdetai.Username = item.seller.username;

                                orderdetai.Title = item.title;
                                _orderEbayService.InsertOrderEbayDetail(orderdetai);
                            }                           

                            return Json(new { success = true, message = "Tạo order thành công" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = false, message ="Xảy ra lỗi, vui lòng thử lại" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Xảy ra lỗi, vui lòng thử lại" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Xảy ra lỗi, vui lòng thử lại" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, message = "Không có sản phẩm nào của ebay trong đơn hàng này" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int id)
        {
            var order = _orderEbayService.GetOrderEbay(id);
            if (order == null)
                return RedirectToAction("ListOrderEbay");

            var orderDetail = _orderEbayService.GetAllOrderEbayDetail(id);

            var model = new OrderEbayModel();
            model.OrderEbay = order;
            model.OrderEbayDetail = orderDetail;

            return View("~/Plugins/Order.Ebay/Views/Edit.cshtml", model);
        }

        #endregion
    }
}
