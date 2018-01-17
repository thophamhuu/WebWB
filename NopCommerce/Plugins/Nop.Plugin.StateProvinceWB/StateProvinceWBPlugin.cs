using Nop.Core.Plugins;
using Nop.Plugin.Worldbuy.StateProvinceWB.Data;
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

namespace Nop.Plugin.Worldbuy.StateProvinceWB
{
    public class StateProvinceWBPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly StateProvinceWBObjectContext _context;
        private readonly ILocalizationService _localizationService;
        public StateProvinceWBPlugin(StateProvinceWBObjectContext context,
            ILocalizationService localizationService
            )
        {
            this._context = context;
            this._localizationService = localizationService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "StateProvinceWB";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.StateProvinceWB.Controllers" },
                { "area", null }
            };
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var groupNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Worldbuy.Plugins");
            if (groupNode == null)
            {
                groupNode = new SiteMapNode()
                {
                    SystemName = "Worldbuy.Plugins",
                    Title = _localizationService.GetResource("Nop.Plugin.Worldbuy"),
                    Visible = true,
                    IconClass = "fa-chain",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                rootNode.ChildNodes.Add(groupNode);

            }

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Worldbuy.Plugin.StateProvinceWB");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Worldbuy.Plugin.StateProvinceWB",
                    Title = _localizationService.GetResource("Nop.Plugin.Worldbuy.StateProvinceWB"),
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                groupNode.ChildNodes.Add(pluginNode);

            }

            var getList = new SiteMapNode()
            {
                SystemName = "Worldbuy.Plugin.StateProvinceWB.List",
                Title = _localizationService.GetResource("Nop.Plugin.Worldbuy.StateProvinceWB.List"),
                ControllerName = "StateProvinceWB",
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
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB", "State Province Postal");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.List", "List");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.Title", "Title");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.STT", "STT");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.PostalCode", "Postal Code");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.Abbreviation", "Abbreviation");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.Update", "Update");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.Delete", "Delete");
            base.Install();
        }

        public override void Uninstall()
        {
            _context.Uninstall();
            this.DeletePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB");
            this.DeletePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.List");
            this.DeletePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.Title");
            this.DeletePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.STT");
            this.DeletePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.PostalCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.Abbreviation");
            this.DeletePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.Update");
            this.DeletePluginLocaleResource("Nop.Plugin.Worldbuy.StateProvinceWB.Delete");
            base.Uninstall();
        }
    }
}
