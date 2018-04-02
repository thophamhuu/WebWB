using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class WorldbuyController : BasePublicController
    {
        #region Fields

        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IOrderReportService _orderReportService;
        private readonly IOrderService _orderService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Constructors

        public WorldbuyController(IProductModelFactory productModelFactory,
            IProductService productService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            ICompareProductsService compareProductsService,
            IWorkflowMessageService workflowMessageService,
            IOrderReportService orderReportService,
            IOrderService orderService,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            CatalogSettings catalogSettings,
            ShoppingCartSettings shoppingCartSettings,
            LocalizationSettings localizationSettings,
            CaptchaSettings captchaSettings)
        {
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._recentlyViewedProductsService = recentlyViewedProductsService;
            this._workflowMessageService = workflowMessageService;
            this._orderReportService = orderReportService;
            this._orderService = orderService;
            this._customerActivityService = customerActivityService;
            this._eventPublisher = eventPublisher;
            this._catalogSettings = catalogSettings;
            this._localizationSettings = localizationSettings;
            this._captchaSettings = captchaSettings;
        }

        #endregion
        //GET: Worldbuy
        [HttpPost, ActionName("ProductReviews")]
        [PublicAntiForgery]
        [FormValueRequired("add-review")]
        [CaptchaValidator]
        public virtual ActionResult ProductReviewAdd(int productId, ProductReviewsModel model)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToRoute("HomePage");

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }

            if (_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing &&
                !_orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id, productId: productId, osIds: new List<int> { (int)OrderStatus.Complete }).Any())
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));

            if (ModelState.IsValid)
            {
                //save review
                int rating = model.AddProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                bool isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Title = model.AddProductReview.Title,
                    ReviewText = model.AddProductReview.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = _storeContext.CurrentStore.Id,
                };
                _productService.InsertProductReview(productReview);

                //update product totals
                _productService.UpdateProductReviewTotals(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                _customerActivityService.InsertActivity("PublicStore.AddProductReview", _localizationService.GetResource("ActivityLog.PublicStore.AddProductReview"), product.Name);

                //raise event
                if (productReview.IsApproved)
                    _eventPublisher.Publish(new ProductReviewApprovedEvent(productReview));

                model.AddProductReview.Title = null;
                model.AddProductReview.ReviewText = null;

                model.AddProductReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddProductReview.Result = _localizationService.GetResource("Reviews.SeeAfterApproving");
                else
                    model.AddProductReview.Result = _localizationService.GetResource("Reviews.SuccessfullyAdded");

                return PartialView(model);
            }

            //If we got this far, something failed, redisplay form
            return PartialView();
        }
    }
}