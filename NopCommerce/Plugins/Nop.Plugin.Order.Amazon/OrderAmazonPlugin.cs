using System;
using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System.Linq;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using Nop.Plugin.Order.Amazon.Data;

namespace Nop.Plugin.Order.Amazon
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class OrderAmazonPlugin : BasePlugin, IAdminMenuPlugin,IMiscPlugin
    {
        private readonly OrderAmazonObjectContext _amazonOrderContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        public OrderAmazonPlugin(OrderAmazonObjectContext amazonOrderContext, ISettingService settingService, ILocalizationService localizationService)
        {
            this._amazonOrderContext = amazonOrderContext;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }


        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _amazonOrderContext.Install();

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon", "Amazon");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.List", "List");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.CartItems", "Cart Items");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.CartId", "Cart Id");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.HMAC", "HMAC");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.ASIN", "ASIN");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.Title", "Title");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.Quantity", "Quantity");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.Price", "Price");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.TotalPrice", "Total Price");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Amazon.Create", "New Cart");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _amazonOrderContext.Uninstall();
            //locales
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon");

            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.List");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.CartItems");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.CartId");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.ASIN");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.HMAC");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.Title");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.Quantity");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.Price");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.TotalPrice");

            this.DeletePluginLocaleResource("Nop.Plugin.Order.Amazon.Create");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {

            var groupNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Order.Plugins");
            if (groupNode == null)
            {
                groupNode = new SiteMapNode()
                {
                    SystemName = "Order.Plugins",
                    Title = _localizationService.GetResource("Nop.Plugin.Order"),
                    Visible = true,
                    IconClass = "fa-chain",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                rootNode.ChildNodes.Add(groupNode);

            }

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Order.Amazon");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Order.Plugins.Amazon",
                    Title = _localizationService.GetResource("Nop.Plugin.Order.Amazon"),
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                groupNode.ChildNodes.Add(pluginNode);

            }
            var configItem = new SiteMapNode()
            {
                SystemName = "Order.Plugins.Amazon.Configure",
                Title = _localizationService.GetResource("Nop.Plugin.Order.Amazon.Setting"),
                Visible = true,
                Url = "/Admin/Plugin/ConfigureMiscPlugin?systemName=Order.Amazon",
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            };

            pluginNode.ChildNodes.Add(configItem);
            var orderList = new SiteMapNode()
            {
                SystemName = "Order.Plugins.Amazon.List",
                Title = _localizationService.GetResource("Nop.Plugin.Order.Amazon.List"),
                ControllerName = "OrderAmazon",
                ActionName = "List",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(orderList);
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "OrderAmazon";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Order.Amazon.Controllers" }, { "area", null } };
        }
    }
}
