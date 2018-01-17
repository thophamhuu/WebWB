using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System.Linq;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using Nop.Plugin.Affiliate.Amazon.Data;
using Nop.Services.Tasks;

namespace Nop.Plugin.Affiliate.Amazon
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class AffiliateAmazonPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly AmazonObjectContext _amazonContext;
        private readonly ISettingService _settingService;
        private readonly AffiliateAmazonSettings _amazonSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IScheduleTaskService _schedualTaskService;
        public AffiliateAmazonPlugin(AmazonObjectContext amazonContext, ISettingService settingService, AffiliateAmazonSettings amazonSettings, ILocalizationService localizationService, IScheduleTaskService schedualTaskService)
        {
            this._amazonContext = amazonContext;
            this._settingService = settingService;
            this._amazonSettings = amazonSettings;
            this._localizationService = localizationService;
            this._schedualTaskService = schedualTaskService;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "AffiliateAmazon";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Affiliate.Amazon.Controllers" }, { "area", null } };
        }


        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _amazonContext.Install();
            var settings = new AffiliateAmazonSettings
            {
                Version = "2013-08-01",
                Endpoint = "webservices.amazon.com",
                Service = "AWSECommerceService"
            };
            _schedualTaskService.InsertTask(new Nop.Core.Domain.Tasks.ScheduleTask()
            {
                Enabled = true,
                Name = "Product Update",
                Seconds = 86400,
                StopOnError = false,
                Type = "Nop.Plugin.Affiliate.Amazon.AffiliateAmazonTask, Nop.Plugin.Affiliate.Amazon"
            });
            _settingService.SaveSetting(settings);
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon", "Amazon");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Sync", "Sync");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Setting", "Setting");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Service", "Service Name");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.AssociateTag", "Associate Tag");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.AWSAccessKeyID", "AWS Access Key ID");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.AWSSecretKey", "AWS Secret Key");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Endpoint", "Endpoint");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Version", "Version");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Durations", "Durations");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Imported", "Imported");

            //Category
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category", "Category");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.CategoryID", "Category ID");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.BrowseNodeID", "Browse Node ID");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.ParentBrowseNodeId", "Parent Browse Node ID");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.IsCategoryRoot", "Is Category Root");



            //Product
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Product", "Product");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item", "Item");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.ASIN", "ASIN");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.ParentASIN", "Parent ASIN");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.DetailUrl", "Detail Url");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.Images", "Images");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.Title", "Title");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.Price", "Price");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.CurrenceCode", "Currence Code");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.CategoryName", "Category Name");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.CompareType", "Type");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _amazonContext.Uninstall();

            //settings
            _settingService.DeleteSetting<AffiliateAmazonSettings>();
            Nop.Core.Domain.Tasks.ScheduleTask task = _schedualTaskService.GetTaskByType("Nop.Plugin.Affiliate.Amazon.AffiliateAmazonTask, Nop.Plugin.Affiliate.Amazon");
            if (task != null)
                _schedualTaskService.DeleteTask(task);
            //locales
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Sync");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Setting");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Service");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.AssociateTag");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.AWSAccessKeyID");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.AWSSecretKey");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Endpoint");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Version");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Durations");

            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Imported");

            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.CategoryID");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.Name");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.ParentCategoryId");

            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Product");

            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.ASIN");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.ParentASIN");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.DetailUrl");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.Images");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.Title");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.Price");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Item.CurrenceCode");

            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.CategoryName");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Amazon.Category.CompareType");

            base.Uninstall();
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

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Affiliate.Plugins.Amazon");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Affiliate.Plugins.Amazon",
                    Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Amazon"),
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                groupNode.ChildNodes.Add(pluginNode);

            }
            var configItem = new SiteMapNode()
            {
                SystemName = "Affiliate.Plugins.Amazon.Configure",
                Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Amazon.Setting"),
                Visible = true,
                Url = "/Admin/Plugin/ConfigureMiscPlugin?systemName=Affiliate.Amazon",
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
            };

            pluginNode.ChildNodes.Add(configItem);

            var getCategoryItem = new SiteMapNode()
            {
                SystemName = "Affiliate.Plugins.Amazon.Category",
                Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Amazon.Category"),
                ControllerName = "AffiliateAmazon",
                ActionName = "Category",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(getCategoryItem);

            var getProductItem = new SiteMapNode()
            {
                SystemName = "Affiliate.Plugins.Amazon.Product",
                Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Amazon.Product"),
                ControllerName = "AffiliateAmazon",
                ActionName = "Product",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(getProductItem);

            var getTaskItem = new SiteMapNode()
            {
                SystemName = "Affiliate.Plugins.Amazon.Task",
                Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Amazon.Task"),
                ControllerName = "AffiliateAmazon",
                ActionName = "Task",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(getTaskItem);

        }

    }
}
