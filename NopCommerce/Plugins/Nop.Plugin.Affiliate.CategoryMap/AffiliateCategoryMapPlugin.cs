using Nop.Core.Data;
using Nop.Core.Plugins;
using Nop.Plugin.Affiliate.CategoryMap;
using Nop.Plugin.Affiliate.CategoryMap.Data;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
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

namespace Nop.Plugin.Affiliate.CategoryMap
{
    public class AffiliateCategoryMapPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private CategoryMappingObjectContext _context;

        public AffiliateCategoryMapPlugin(ISettingService settingService, CategoryMappingObjectContext context, ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._context = context;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ProductMapping";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Affiliate.CategoryMap.Controllers" }, { "area", null } }; 
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _context.Install();
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate", "Affiliates");

            var categorySettings = new ProductMappingSettings
            {
                AdditionalCostPercent = 5
            };
            _settingService.SaveSetting(categorySettings);

            base.Install();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var groupNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Affiliate.Plugins");
            if (groupNode == null)
            {
                groupNode = new SiteMapNode()
                {
                    SystemName = "Affiliate.Plugins",
                    Title = _localizationService.GetResource("Nop.Plugin.Affiliate"),
                    Visible = true,
                    IconClass = "fa-chain",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                rootNode.ChildNodes.Add(groupNode);
            }

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Affiliate.Plugin.CategoryMap");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Affiliate.Plugin.CategoryMap",
                    Title = "Danh mục liên kết",
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                groupNode.ChildNodes.Add(pluginNode);

            }
            var configItem = new SiteMapNode()
            {
                SystemName = "Affiliate.Plugins.Amazon.Configure",
                Title = "Cài đặt",
                Visible = true,
                Url = "/Admin/Plugin/ConfigureMiscPlugin?systemName=Affiliate.CategoryMap",
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            };

            pluginNode.ChildNodes.Add(configItem);

        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _context.Uninstall();
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate");

            _settingService.DeleteSetting<ProductMappingSettings>();
            base.Uninstall();
        }
    }
}
