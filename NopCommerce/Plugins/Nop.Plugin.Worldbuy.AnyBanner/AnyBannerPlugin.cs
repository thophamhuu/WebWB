using Newtonsoft.Json;
using Nop.Core.Plugins;
using Nop.Plugin.Worldbuy.AnyBanner.Data;
using Nop.Plugin.Worldbuy.AnyBanner.Services;
using Nop.Services.Cms;
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
using System.Xml;

namespace Nop.Plugin.Worldbuy.AnyBanner
{
    public class AnyBannerPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly AnyBannerObjectContext _context;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWB_AnyBannerService _AnyBannerService;
        public AnyBannerPlugin(AnyBannerObjectContext context, ISettingService settingService,
            IWB_AnyBannerService AnyBannerService,
            ILocalizationService localizationService
            )
        {
            this._context = context;
            this._settingService = settingService;
            this._AnyBannerService = AnyBannerService;
            this._localizationService = localizationService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "AnyBanner";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Worldbuy.AnyBanner.Controllers" },
                { "area", null }
            };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "AnyBanner";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Worldbuy.AnyBanner.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone},
            };
        }

        public IList<string> GetWidgetZones()
        {
            return _AnyBannerService.GetWidgetZones().Select(x => x.Value).ToList();
        }
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var groupNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Worldbuy.Plugins");
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Worldbuy.Plugin.AnyBanner");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Worldbuy.Plugin.AnyBanner",
                    Title = _localizationService.GetResource("Nop.Plugin.Worldbuy.AnyBanner"),
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                groupNode.ChildNodes.Add(pluginNode);

            }

            var getList = new SiteMapNode()
            {
                SystemName = "Worldbuy.Plugin.AnyBanner.List",
                Title = _localizationService.GetResource("Nop.Plugin.Worldbuy.AnyBanner.List"),
                ControllerName = "AnyBanner",
                ActionName = "List",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(getList);
        }
        public override void Install()
        {
            _context.Install();
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.imageurl", "Image Url");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.url", "Url");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.image", "Image");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.title", "Title");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.isactived", "Is Actived");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.order", "Order");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.items", "Banner Items");

            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.settings", "Settings");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.widgetzone", "Widget Zone");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.isactived", "Is Actived");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.name", "Name");


            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.List", "List");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.AnyBanner", "Any Banner");
            base.Install();
        }

        public override void Uninstall()
        {
            _context.Uninstall();

            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.imageurl");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.url");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.image");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.title");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.isactived");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.item.order");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.items");

            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.settings");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.widgetzone");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.isactived");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.name");


            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner.List");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.AnyBanner");

            base.Uninstall();
        }
    }
}
