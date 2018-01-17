using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Order.Amazon.Domain;
using Nop.Plugin.Order.Amazon.Models;
using Nop.Plugin.Order.Amazon.Service;
using Nop.Services;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Order.Amazon.Controllers
{
    [AdminAuthorize]
    public class OrderAmazonController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;

        private readonly IOrderService _orderService;
        private readonly IOrderReportService _orderReportService;
        private readonly IVendorService _vendorService;
        private readonly ICacheManager _cacheManager;
        private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly ICountryService _countryService;
        private readonly IProductService _productService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly TaxSettings _taxSettings;
        private readonly IAffiliateService _affiliateService;
        private readonly IDiscountService _discountService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly AddressSettings _addressSettings;

        private readonly IOrderAmazonService _orderAmazonService;

        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderAmazon> _orderAmazonRepository;
        private readonly IRepository<ProductMapping> _productMappingRepository;

        private readonly IProductAttributeParser _productAttributeParser;
        public OrderAmazonController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper,

            IOrderService orderService,
            IOrderReportService orderReportService,
            IVendorService vendorService,
            ICacheManager cacheManager,
            IShippingService shippingService,
            IPaymentService paymentService,
            ICountryService countryService,
            IProductService productService,
            IPriceFormatter priceFormatter,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            TaxSettings taxSettings,
            IAffiliateService affiliateService,
            IDiscountService discountService,
            IAddressAttributeFormatter addressAttributeFormatter,
            AddressSettings addressSettings,

            IOrderAmazonService orderAmazonService,

            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderAmazon> orderAmazonRepository,
            IRepository<ProductMapping> productMappingRepository,

            IProductAttributeParser productAttributeParser,
        IPluginFinder pluginFinder)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._dateTimeHelper = dateTimeHelper;

            this._orderService = orderService;
            this._orderReportService = orderReportService;
            this._orderAmazonService = orderAmazonService;
            this._vendorService = vendorService;
            this._cacheManager = cacheManager;
            this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._countryService = countryService;
            this._productService = productService;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;

            this._taxSettings = taxSettings;
            this._affiliateService = affiliateService;
            this._discountService = discountService;
            this._addressAttributeFormatter = addressAttributeFormatter;
            this._addressSettings = addressSettings;

            this._orderItemRepository = orderItemRepository;
            this._orderAmazonRepository = orderAmazonRepository;
            this._productMappingRepository = productMappingRepository;

            this._productAttributeParser = productAttributeParser;
        }
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<OrderAmazonSettings>(storeScope);
            var model = new ConfigurationModel()
            {
                ActiveStoreScopeConfiguration = storeScope,
                AssociateTag = amazonSettings.AssociateTag,
                AWSAccessKeyID = amazonSettings.AWSAccessKeyID,
                AWSSecretKey = amazonSettings.AWSSecretKey,
                Version = amazonSettings.Version,
                Endpoint = amazonSettings.Endpoint,
                Service = amazonSettings.Service,
            };
            return View("~/Plugins/Order.Amazon/Views/Configure.cshtml", model);
        }
        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<OrderAmazonSettings>(storeScope);

            amazonSettings.AssociateTag = model.AssociateTag;
            amazonSettings.AWSAccessKeyID = model.AWSAccessKeyID;
            amazonSettings.AWSSecretKey = model.AWSSecretKey;
            amazonSettings.Version = model.Version;
            amazonSettings.Service = model.Service;
            amazonSettings.Endpoint = model.Endpoint;
            model.ActiveStoreScopeConfiguration = storeScope;

            _settingService.SaveSetting(amazonSettings);

            _settingService.ClearCache();
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
        public virtual ActionResult List(
            [ModelBinder(typeof(CommaSeparatedModelBinder))] List<string> orderStatusIds = null,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] List<string> paymentStatusIds = null,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] List<string> shippingStatusIds = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //order statuses
            var model = new Admin.Models.Orders.OrderListModel();
            model.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableOrderStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            if (orderStatusIds != null && orderStatusIds.Any())
            {
                foreach (var item in model.AvailableOrderStatuses.Where(os => orderStatusIds.Contains(os.Value)))
                    item.Selected = true;
                model.AvailableOrderStatuses.First().Selected = false;
            }
            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            if (paymentStatusIds != null && paymentStatusIds.Any())
            {
                foreach (var item in model.AvailablePaymentStatuses.Where(ps => paymentStatusIds.Contains(ps.Value)))
                    item.Selected = true;
                model.AvailablePaymentStatuses.First().Selected = false;
            }

            //shipping statuses
            model.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.AvailableShippingStatuses.Insert(0, new SelectListItem
            { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0", Selected = true });
            if (shippingStatusIds != null && shippingStatusIds.Any())
            {
                foreach (var item in model.AvailableShippingStatuses.Where(ss => shippingStatusIds.Contains(ss.Value)))
                    item.Selected = true;
                model.AvailableShippingStatuses.First().Selected = false;
            }

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var w in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = w.Name, Value = w.Id.ToString() });

            //payment methods
            model.AvailablePaymentMethods.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "" });
            foreach (var pm in _paymentService.LoadAllPaymentMethods())
                model.AvailablePaymentMethods.Add(new SelectListItem { Text = pm.PluginDescriptor.FriendlyName, Value = pm.PluginDescriptor.SystemName });

            //billing countries
            foreach (var c in _countryService.GetAllCountriesForBilling(showHidden: true))
            {
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }
            model.AvailableCountries.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //a vendor should have access only to orders with his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            return View("~/Plugins/Order.Amazon/Views/List.cshtml", model);
        }

        [HttpPost]
        public virtual ActionResult OrderList(DataSourceRequest command, Admin.Models.Orders.OrderListModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<OrderAmazonSettings>(storeScope);
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatusIds = !model.OrderStatusIds.Contains(0) ? model.OrderStatusIds : null;
            var paymentStatusIds = !model.PaymentStatusIds.Contains(0) ? model.PaymentStatusIds : null;
            var shippingStatusIds = !model.ShippingStatusIds.Contains(0) ? model.ShippingStatusIds : null;

            var filterByProductId = 0;
            var product = _productService.GetProductById(model.ProductId);
            if (product != null && HasAccessToProduct(product))
                filterByProductId = model.ProductId;

            //load orders
            var orders = _orderAmazonService.SearchOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
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
                        CustomOrderNumber = x.CustomOrderNumber,

                        OrderAmazon = _orderAmazonService.GetOrderAmazonByOrderId(x.Id)
                    };
                }),
                Total = orders.TotalCount
            };

            //summary report
            //currently we do not support productId and warehouseId parameters for this report
            var reportSummary = _orderReportService.GetOrderAverageReportLine(
                storeId: model.StoreId,
                vendorId: model.VendorId,
                orderId: 0,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            var profit = _orderReportService.ProfitReport(
                storeId: model.StoreId,
                vendorId: model.VendorId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                startTimeUtc: startDateValue,
                endTimeUtc: endDateValue,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);
            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");

            gridModel.ExtraData = new Admin.Models.Orders.OrderAggreratorModel
            {
                aggregatorprofit = _priceFormatter.FormatPrice(profit, true, false),
                aggregatorshipping = _priceFormatter.FormatShippingPrice(reportSummary.SumShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false),
                aggregatortax = _priceFormatter.FormatPrice(reportSummary.SumTax, true, false),
                aggregatortotal = _priceFormatter.FormatPrice(reportSummary.SumOrders, true, false)
            };

            return Json(gridModel);
        }
        public ActionResult Order(int id)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<OrderAmazonSettings>(storeScope);
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted)
                //No order found with the specified id
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            var model = new OrderModel();
            PrepareOrderDetailsModel(model, order);
            model.OrderAmazon = _orderAmazonService.GetOrderAmazonByOrderId(model.Id);
            var warnings = TempData["nop.admin.order.warnings"] as List<string>;
            if (warnings != null)
                model.Warnings = warnings;

            return View("~/Plugins/Order.Amazon/Views/Order.cshtml", model);
        }
        [HttpGet]
        public ActionResult CartCreate(int orderId)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<OrderAmazonSettings>(storeScope);
            var orderAmazon = _orderAmazonRepository.TableNoTracking.FirstOrDefault(x => x.OrderId == orderId);
            if (orderAmazon == null)
            {
                var order = _orderService.GetOrderById(orderId);

                if (order != null)
                {
                    var model = new OrderModel();
                    PrepareOrderDetailsModel(model, order);
                    List<Item> items = new List<Item>();
                    int i = 0;
                    foreach (var item in model.Items)
                    {
                        var productMapping = _productMappingRepository.TableNoTracking.FirstOrDefault(x => x.ProductId == item.ProductId);
                        if (productMapping != null)
                        {
                            i++;
                            items.Add(new Item
                            {
                                ASIN = item.Sku != null ? item.Sku : productMapping.ProductSourceId,
                                Quantity = i
                            });
                        }
                    }
                    if (i > 0)
                    {
                        var cartAmazon = _orderAmazonService.CartCreate(amazonSettings, items);
                        if (cartAmazon != null)
                        {

                            if (cartAmazon.Cart != null && cartAmazon.Cart.CartId != null)
                            {
                                orderAmazon = new OrderAmazon
                                {
                                    OrderId = orderId,
                                    CartId = cartAmazon.Cart.CartId,
                                    CreateOrder = DateTime.Now,
                                    HMAC = cartAmazon.Cart.HMAC,
                                    PurchaseURL = cartAmazon.Cart.PurchaseURL
                                };
                                try
                                {
                                    _orderAmazonRepository.Insert(orderAmazon);
                                    return RedirectToAction("Order", new { id = orderId });
                                }
                                catch (Exception ex)
                                {
                                    LogException(ex);
                                }
                            }
                            else
                            {
                                ErrorNotification("The item is not eligible to be added to the cart!");
                            }
                        }
                    }

                }
            }
            return RedirectToAction("List");
        }
        [HttpPost]
        public ActionResult CartGet(int orderAmazonId)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<OrderAmazonSettings>(storeScope);
            try
            {
                var orderAmazon = _orderAmazonRepository.TableNoTracking.FirstOrDefault(x => x.Id == orderAmazonId);
                if (orderAmazon != null)
                {
                    var response = _orderAmazonService.CartGet(amazonSettings, orderAmazon.CartId, orderAmazon.HMAC);
                    if (response.Cart != null)
                    {
                        if (response.Cart.CartItems != null)
                        {
                            var items = response.Cart.CartItems.ToList();
                            items.ForEach(x =>
                            {
                                x.OrderAmazonId = orderAmazonId;
                            });
                            response.Cart.CartItems = items.ToArray();
                        }
                        else
                        {
                            response.Cart.CartItems = new List<CartItem>().ToArray();
                        }
                        return Json(response.Cart);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);

            }
            return Json(new Cart());
        }
        [HttpPost]
        public ActionResult CartAdd()
        {
            return Json(new { });
        }
        [HttpPost]
        public ActionResult CartModify(int OrderAmazonId, string CartItemId, string ASIN, int Quantity)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<OrderAmazonSettings>(storeScope);
            var orderAmazon = _orderAmazonRepository.Table.FirstOrDefault(x => x.Id == OrderAmazonId);
            if (orderAmazon != null)
            {

                var productMapping = _productMappingRepository.Table.FirstOrDefault(x => x.ProductSourceId == ASIN);
                if (productMapping != null)
                {
                    var productDetail = _orderItemRepository.Table.FirstOrDefault(x => x.ProductId == productMapping.ProductId && x.OrderId == orderAmazon.OrderId);
                    if (productDetail != null)
                    {
                        productDetail.Quantity = Quantity;
                        _orderItemRepository.Update(productDetail);
                        var item = new Item
                        {
                            CartItemId = CartItemId,
                            Quantity = Quantity
                        };
                        _orderAmazonService.CartModify(amazonSettings, orderAmazon.CartId, orderAmazon.HMAC, new List<Item> { item });
                    }
                }
            }
            return Json(new { });
        }
        [HttpPost]
        public ActionResult CartDelete(int OrderAmazonId, string CartItemId, string ASIN)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var amazonSettings = _settingService.LoadSetting<OrderAmazonSettings>(storeScope);
            var orderAmazon = _orderAmazonRepository.Table.FirstOrDefault(x => x.Id == OrderAmazonId);
            if (orderAmazon != null)
            {

                var productMapping = _productMappingRepository.Table.FirstOrDefault(x => x.ProductSourceId == ASIN);
                if (productMapping != null)
                {
                    var productDetail = _orderItemRepository.Table.FirstOrDefault(x => x.ProductId == productMapping.ProductId && x.OrderId == orderAmazon.OrderId);
                    if (productDetail != null)
                    {
                        _orderItemRepository.Delete(productDetail);
                        var item = new Item
                        {
                            CartItemId = CartItemId,
                            Quantity = 0
                        };
                        _orderAmazonService.CartModify(amazonSettings, orderAmazon.CartId, orderAmazon.HMAC, new List<Item> { item });
                    }
                }
            }
            return Json(new { });
        }

        protected virtual ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
        }
        protected JsonResult AccessDeniedKendoGridJson()
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            return ErrorForKendoGridJson(localizationService.GetResource("Admin.AccessDenied.Description"));
        }


        [NonAction]
        protected virtual bool HasAccessToProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return product.VendorId == vendorId;
        }

        [NonAction]
        protected virtual bool HasAccessToOrder(Nop.Core.Domain.Orders.Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            var hasVendorProducts = order.OrderItems.Any(orderItem => orderItem.Product.VendorId == vendorId);
            return hasVendorProducts;
        }

        [NonAction]
        protected virtual void PrepareOrderDetailsModel(OrderModel model, Nop.Core.Domain.Orders.Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (model == null)
                throw new ArgumentNullException("model");

            model.Id = order.Id;
            model.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.OrderStatusId = order.OrderStatusId;
            model.OrderGuid = order.OrderGuid;
            model.CustomOrderNumber = order.CustomOrderNumber;
            var store = _storeService.GetStoreById(order.StoreId);
            model.StoreName = store != null ? store.Name : "Unknown";
            model.CustomerId = order.CustomerId;
            var customer = order.Customer;
            model.CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.CustomerIp = order.CustomerIp;
            model.VatNumber = order.VatNumber;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.AllowCustomersToSelectTaxDisplayType = _taxSettings.AllowCustomersToSelectTaxDisplayType;
            model.TaxDisplayType = _taxSettings.TaxDisplayType;

            var affiliate = _affiliateService.GetAffiliateById(order.AffiliateId);
            if (affiliate != null)
            {
                model.AffiliateId = affiliate.Id;
                model.AffiliateName = affiliate.GetFullName();
            }

            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            //custom values
            model.CustomValues = order.DeserializeCustomValues();

            #region Order totals

            var primaryStoreCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (primaryStoreCurrency == null)
                throw new Exception("Cannot load primary store currency");

            //subtotal
            model.OrderSubtotalInclTax = _priceFormatter.FormatPrice(order.OrderSubtotalInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.OrderSubtotalExclTax = _priceFormatter.FormatPrice(order.OrderSubtotalExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.OrderSubtotalInclTaxValue = order.OrderSubtotalInclTax;
            model.OrderSubtotalExclTaxValue = order.OrderSubtotalExclTax;
            //discount (applied to order subtotal)
            string orderSubtotalDiscountInclTaxStr = _priceFormatter.FormatPrice(order.OrderSubTotalDiscountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            string orderSubtotalDiscountExclTaxStr = _priceFormatter.FormatPrice(order.OrderSubTotalDiscountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            if (order.OrderSubTotalDiscountInclTax > decimal.Zero)
                model.OrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
                model.OrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;
            model.OrderSubTotalDiscountInclTaxValue = order.OrderSubTotalDiscountInclTax;
            model.OrderSubTotalDiscountExclTaxValue = order.OrderSubTotalDiscountExclTax;

            //shipping
            model.OrderShippingInclTax = _priceFormatter.FormatShippingPrice(order.OrderShippingInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
            model.OrderShippingExclTax = _priceFormatter.FormatShippingPrice(order.OrderShippingExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            model.OrderShippingInclTaxValue = order.OrderShippingInclTax;
            model.OrderShippingExclTaxValue = order.OrderShippingExclTax;

            //payment method additional fee
            if (order.PaymentMethodAdditionalFeeInclTax > decimal.Zero)
            {
                model.PaymentMethodAdditionalFeeInclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true);
                model.PaymentMethodAdditionalFeeExclTax = _priceFormatter.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false);
            }
            model.PaymentMethodAdditionalFeeInclTaxValue = order.PaymentMethodAdditionalFeeInclTax;
            model.PaymentMethodAdditionalFeeExclTaxValue = order.PaymentMethodAdditionalFeeExclTax;


            //tax
            model.Tax = _priceFormatter.FormatPrice(order.OrderTax, true, false);
            SortedDictionary<decimal, decimal> taxRates = order.TaxRatesDictionary;
            bool displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
            bool displayTax = !displayTaxRates;
            foreach (var tr in order.TaxRatesDictionary)
            {
                model.TaxRates.Add(new OrderModel.TaxRate
                {
                    Rate = _priceFormatter.FormatTaxRate(tr.Key),
                    Value = _priceFormatter.FormatPrice(tr.Value, true, false),
                });
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.TaxValue = order.OrderTax;
            model.TaxRatesValue = order.TaxRates;

            //discount
            if (order.OrderDiscount > 0)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-order.OrderDiscount, true, false);
            model.OrderTotalDiscountValue = order.OrderDiscount;

            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new OrderModel.GiftCard
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-gcuh.UsedValue, true, false),
                });
            }

            //reward points
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-order.RedeemedRewardPointsEntry.UsedAmount, true, false);
            }

            //total
            model.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);
            model.OrderTotalValue = order.OrderTotal;

            //refunded amount
            if (order.RefundedAmount > decimal.Zero)
                model.RefundedAmount = _priceFormatter.FormatPrice(order.RefundedAmount, true, false);

            //used discounts
            var duh = _discountService.GetAllDiscountUsageHistory(orderId: order.Id);
            foreach (var d in duh)
            {
                model.UsedDiscounts.Add(new OrderModel.UsedDiscountModel
                {
                    DiscountId = d.DiscountId,
                    DiscountName = d.Discount.Name
                });
            }

            //profit (hide for vendors)
            if (_workContext.CurrentVendor == null)
            {
                var profit = _orderReportService.ProfitReport(orderId: order.Id);
                model.Profit = _priceFormatter.FormatPrice(profit, true, false);
            }

            #endregion

            #region Payment info


            //payment method info
            var pm = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = pm != null ? pm.PluginDescriptor.FriendlyName : order.PaymentMethodSystemName;
            model.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);

            #endregion

            #region Billing & shipping info

            model.BillingAddress = order.BillingAddress.ToModel();
            model.BillingAddress.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(order.BillingAddress.CustomAttributes);
            model.BillingAddress.FirstNameEnabled = true;
            model.BillingAddress.FirstNameRequired = true;
            model.BillingAddress.LastNameEnabled = true;
            model.BillingAddress.LastNameRequired = true;
            model.BillingAddress.EmailEnabled = true;
            model.BillingAddress.EmailRequired = true;
            model.BillingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.BillingAddress.CompanyRequired = _addressSettings.CompanyRequired;
            model.BillingAddress.CountryEnabled = _addressSettings.CountryEnabled;
            model.BillingAddress.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
            model.BillingAddress.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
            model.BillingAddress.CityEnabled = _addressSettings.CityEnabled;
            model.BillingAddress.CityRequired = _addressSettings.CityRequired;
            model.BillingAddress.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.BillingAddress.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.BillingAddress.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.BillingAddress.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.BillingAddress.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.BillingAddress.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.BillingAddress.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.BillingAddress.PhoneRequired = _addressSettings.PhoneRequired;
            model.BillingAddress.FaxEnabled = _addressSettings.FaxEnabled;
            model.BillingAddress.FaxRequired = _addressSettings.FaxRequired;

            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext); ;
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;

                model.PickUpInStore = order.PickUpInStore;
                if (!order.PickUpInStore)
                {
                    model.ShippingAddress = order.ShippingAddress.ToModel();
                    model.ShippingAddress.FormattedCustomAddressAttributes = _addressAttributeFormatter.FormatAttributes(order.ShippingAddress.CustomAttributes);
                    model.ShippingAddress.FirstNameEnabled = true;
                    model.ShippingAddress.FirstNameRequired = true;
                    model.ShippingAddress.LastNameEnabled = true;
                    model.ShippingAddress.LastNameRequired = true;
                    model.ShippingAddress.EmailEnabled = true;
                    model.ShippingAddress.EmailRequired = true;
                    model.ShippingAddress.CompanyEnabled = _addressSettings.CompanyEnabled;
                    model.ShippingAddress.CompanyRequired = _addressSettings.CompanyRequired;
                    model.ShippingAddress.CountryEnabled = _addressSettings.CountryEnabled;
                    model.ShippingAddress.CountryRequired = _addressSettings.CountryEnabled; //country is required when enabled
                    model.ShippingAddress.StateProvinceEnabled = _addressSettings.StateProvinceEnabled;
                    model.ShippingAddress.CityEnabled = _addressSettings.CityEnabled;
                    model.ShippingAddress.CityRequired = _addressSettings.CityRequired;
                    model.ShippingAddress.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
                    model.ShippingAddress.StreetAddressRequired = _addressSettings.StreetAddressRequired;
                    model.ShippingAddress.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
                    model.ShippingAddress.StreetAddress2Required = _addressSettings.StreetAddress2Required;
                    model.ShippingAddress.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
                    model.ShippingAddress.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
                    model.ShippingAddress.PhoneEnabled = _addressSettings.PhoneEnabled;
                    model.ShippingAddress.PhoneRequired = _addressSettings.PhoneRequired;
                    model.ShippingAddress.FaxEnabled = _addressSettings.FaxEnabled;
                    model.ShippingAddress.FaxRequired = _addressSettings.FaxRequired;

                    model.ShippingAddressGoogleMapsUrl = string.Format("http://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={0}", Server.UrlEncode(order.ShippingAddress.Address1 + " " + order.ShippingAddress.ZipPostalCode + " " + order.ShippingAddress.City + " " + (order.ShippingAddress.Country != null ? order.ShippingAddress.Country.Name : "")));
                }
                else
                {
                    if (order.PickupAddress != null)
                    {
                        model.PickupAddress = order.PickupAddress.ToModel();
                        model.PickupAddressGoogleMapsUrl = string.Format("http://maps.google.com/maps?f=q&hl=en&ie=UTF8&oe=UTF8&geocode=&q={0}",
                            Server.UrlEncode(string.Format("{0} {1} {2} {3}", order.PickupAddress.Address1, order.PickupAddress.ZipPostalCode, order.PickupAddress.City,
                                order.PickupAddress.Country != null ? order.PickupAddress.Country.Name : string.Empty)));
                    }
                }
                model.ShippingMethod = order.ShippingMethod;
            }

            #endregion

            #region Products

            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;
            bool hasDownloadableItems = false;
            var products = order.OrderItems;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                products = products
                    .Where(orderItem => orderItem.Product.VendorId == _workContext.CurrentVendor.Id)
                    .ToList();
            }
            foreach (var orderItem in products)
            {
                if (orderItem.Product.IsDownload)
                    hasDownloadableItems = true;

                var orderItemModel = new OrderModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.Name,
                    Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
                    Quantity = orderItem.Quantity,
                    IsDownload = orderItem.Product.IsDownload,
                    DownloadCount = orderItem.DownloadCount,
                    DownloadActivationType = orderItem.Product.DownloadActivationType,
                    IsDownloadActivated = orderItem.IsDownloadActivated
                };


                //vendor
                var vendor = _vendorService.GetVendorById(orderItem.Product.VendorId);
                orderItemModel.VendorName = vendor != null ? vendor.Name : "";

                //unit price
                orderItemModel.UnitPriceInclTaxValue = orderItem.UnitPriceInclTax;
                orderItemModel.UnitPriceExclTaxValue = orderItem.UnitPriceExclTax;
                orderItemModel.UnitPriceInclTax = _priceFormatter.FormatPrice(orderItem.UnitPriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.UnitPriceExclTax = _priceFormatter.FormatPrice(orderItem.UnitPriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                //discounts
                orderItemModel.DiscountInclTaxValue = orderItem.DiscountAmountInclTax;
                orderItemModel.DiscountExclTaxValue = orderItem.DiscountAmountExclTax;
                orderItemModel.DiscountInclTax = _priceFormatter.FormatPrice(orderItem.DiscountAmountInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.DiscountExclTax = _priceFormatter.FormatPrice(orderItem.DiscountAmountExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);
                //subtotal
                orderItemModel.SubTotalInclTaxValue = orderItem.PriceInclTax;
                orderItemModel.SubTotalExclTaxValue = orderItem.PriceExclTax;
                orderItemModel.SubTotalInclTax = _priceFormatter.FormatPrice(orderItem.PriceInclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, true, true);
                orderItemModel.SubTotalExclTax = _priceFormatter.FormatPrice(orderItem.PriceExclTax, true, primaryStoreCurrency, _workContext.WorkingLanguage, false, true);

                orderItemModel.AttributeInfo = orderItem.AttributeDescription;
                if (orderItem.Product.IsRecurring)
                    orderItemModel.RecurringInfo = string.Format(_localizationService.GetResource("Admin.Orders.Products.RecurringPeriod"), orderItem.Product.RecurringCycleLength, orderItem.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));
                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                model.Items.Add(orderItemModel);
            }
            model.HasDownloadableProducts = hasDownloadableItems;
            #endregion
        }
    }
}
