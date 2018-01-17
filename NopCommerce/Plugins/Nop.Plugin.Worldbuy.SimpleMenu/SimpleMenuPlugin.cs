using Newtonsoft.Json;
using Nop.Core.Plugins;
using Nop.Plugin.Worldbuy.SimpleMenu.Data;
using Nop.Plugin.Worldbuy.SimpleMenu.Services;
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

namespace Nop.Plugin.Worldbuy.SimpleMenu
{
    public class SimpleMenuPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly SimpleMenuObjectContext _context;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWB_SimpleMenuService _simpleMenuService;
        public SimpleMenuPlugin(SimpleMenuObjectContext context, ISettingService settingService,
            IWB_SimpleMenuService simpleMenuService,
            ILocalizationService localizationService)
        {
            this._context = context;
            this._settingService = settingService;
            this._simpleMenuService = simpleMenuService;
            this._localizationService = localizationService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "SimpleMenu";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" },
                { "area", null }
            };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "SimpleMenu";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Worldbuy.SimpleMenu.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone},
            };
        }

        public IList<string> GetWidgetZones()
        {
            return _simpleMenuService.GetWidgetZones().Select(x => x.Value).ToList();
        }
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var groupNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Worldbuy.Plugins");
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Worldbuy.Plugin.SimpleMenu");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Worldbuy.Plugin.SimpleMenu",
                    Title = _localizationService.GetResource("Nop.Plugin.Worldbuy.SimpleMenu"),
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                groupNode.ChildNodes.Add(pluginNode);

            }

            var getList = new SiteMapNode()
            {
                SystemName = "Worldbuy.Plugin.SimpleMenu.List",
                Title = _localizationService.GetResource("Nop.Plugin.Worldbuy.SimpleMenu.List"),
                ControllerName = "SimpleMenu",
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
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.iconimageurl", "Icon Image Url");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.url", "Url");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.image", "Image");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.title", "Title");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.order", "Order");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.items", "Simple Menu Items");

            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.settings", "Settings");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.widgetzone", "Widget Zone");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.name", "Name");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.icontoright", "Icon To Right");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.useiconimage", "Use Icon Image");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.isactived", "Is Actived");


            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu.List", "List Of Simple Menu");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.simplemenu", "Simple Menu");
            base.Install();
        }

        public override void Uninstall()
        {
            _context.Uninstall();

            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.iconimageurl");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.url");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.image");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.title");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.item.order");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.items");

            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.settings");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.widgetzone");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.name");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.icontoright");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.useiconimage");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.isactived");


            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu.List");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.simplemenu");

            base.Uninstall();
        }
    }
}
