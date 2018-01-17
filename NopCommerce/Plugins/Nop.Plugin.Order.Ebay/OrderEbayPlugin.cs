using Nop.Core.Plugins;
using Nop.Plugin.Order.Ebay.Data;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Order.Ebay
{
    public class OrderEbayPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private OrderEbayObjectContext _context;

        #endregion

        #region Ctor

        public OrderEbayPlugin(ISettingService settingService, ILocalizationService localizationService, OrderEbayObjectContext context)
        {
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _context.Install();

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order", "Order Plugin");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Ebay", "Order Ebay");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Ebay.ListOrder", "List Order");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Order.Ebay.ListOrderEbay", "List Order Ebay");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _context.Uninstall();

            this.DeletePluginLocaleResource("Nop.Plugin.Order");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Ebay");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Ebay.ListOrder");
            this.DeletePluginLocaleResource("Nop.Plugin.Order.Ebay.ListOrderEbay");

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
                    IconClass = "fa-opencart",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                rootNode.ChildNodes.Add(groupNode);

            }

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Order.Ebay");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Order.Ebay",
                    Title = _localizationService.GetResource("Nop.Plugin.Order.Ebay"),
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                groupNode.ChildNodes.Add(pluginNode);

            }

            var listOrder = new SiteMapNode()
            {
                SystemName = "Order.Ebay.ListOrder",
                Title = _localizationService.GetResource("Nop.Plugin.Order.Ebay.ListOrder"),
                ControllerName = "OrderEbay",
                ActionName = "ListOrder",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(listOrder);

            var listOrderEbay = new SiteMapNode()
            {
                SystemName = "Order.Ebay.ListOrderEbay",
                Title = _localizationService.GetResource("Nop.Plugin.Order.Ebay.ListOrderEbay"),
                ControllerName = "OrderEbay",
                ActionName = "ListOrderEbay",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(listOrderEbay);
        }

        #endregion
    }
}
