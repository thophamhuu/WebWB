using Nop.Core.Plugins;
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

namespace Nop.Plugin.Worldbuy
{
    public class AnyBannerPlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        public AnyBannerPlugin(ISettingService settingService,
            ILocalizationService localizationService
            )
        {
            this._settingService = settingService;
            this._localizationService = localizationService;
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
                    IconClass = "fa-globe",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                rootNode.ChildNodes.Add(groupNode);
            }
        }
        public override void Install()
        {
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy", "Worldbuy Plugin");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.edit", "Edit");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.delete", "Delete");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.create", "Create");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.save", "Save");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.savecontinue", "Save And Continue");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.backtolist", "Back To List");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.addnew", "Add New");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.enabled", "Enabled");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.disabled", "Disabled");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.confirm", "Confirm");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.view", "View");

            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.columnonrow_1280", "Column On Row (1280)");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.columnonrow_1000", "Column On Row (100)");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.columnonrow_768", "Column On Row (768)");
            this.AddOrUpdatePluginLocaleResource("nop.plugin.worldbuy.columnonrow_480", "Column On Row (480)");
            base.Install();
        }

        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("nop.plugin.worldbuy");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.edit");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.delete");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.create");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.save");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.savecontinue");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.backtolist");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.addnew");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.enabled");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.disabled");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.confirm");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.view");

            this.DeletePluginLocaleResource("nop.plugin.worldbuy.columnonrow_1280");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.columnonrow_1000");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.columnonrow_768");
            this.DeletePluginLocaleResource("nop.plugin.worldbuy.columnonrow_480");
            base.Uninstall();
        }
    }
}
