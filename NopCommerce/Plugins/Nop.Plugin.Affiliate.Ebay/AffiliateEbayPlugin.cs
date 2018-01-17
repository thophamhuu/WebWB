using eBay.Service.Call;
using eBay.Service.Core.Sdk;
using eBay.Service.Core.Soap;
using Nop.Core.Plugins;
using Nop.Plugin.Affiliate.Ebay.Data;
using Nop.Plugin.Affiliate.Ebay.Domain;
using Nop.Plugin.Affiliate.Ebay.Models;
using Nop.Plugin.Affiliate.Ebay.Services;
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

namespace Nop.Plugin.Affiliate.Ebay
{
    public class AffiliateEbayPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

            private readonly ISettingService _settingService;
            private CategoryEbayObjectContext _context;
            private readonly IAffiliateEbayService _affiliateEbayService;
            private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public AffiliateEbayPlugin(ISettingService settingService, CategoryEbayObjectContext context, IAffiliateEbayService affiliateEbayService,
            ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._context = context;
            this._affiliateEbayService = affiliateEbayService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "AffiliateEbay";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Affiliate.Ebay.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _context.Install();

            var settings = new ConfigurationModel
            {
                AppID = "PhuocPha-worldbuy-PRD-28e35c535-94b2bb4b",
                DevID = "4343ce44-efa1-4c33-a431-18daed74c054",
                Token = "v^1.1#i^1#p^1#r^0#I^3#f^0#t^H4sIAAAAAAAAAOVXa2wUVRTu9oVIizESwYK4nRZjkJm98+ruDt2FpS20aWkXtiBQCZnHHXZgdmYzc5ftxjTWGpr4gB/+MPFFahRJE8WKSg3aRCQEoeAfNfEBPzQQrMQYeQQjwXhndinbSngWIXH/bObcM+ee7zvfOXcu6C6dPLe3sfd8uWdSYV836C70eOgpYHJpyeNTiworSgpAnoOnr7u6u7in6JdaW0zoSWE5tJOmYUNvZ0I3bME1hoiUZQimaGu2YIgJaAtIFmKRpS0CQwEhaZnIlE2d8DbVhwiOlRiFkThallRIcwq2GpditpshgpFYKShyNTDA8KzEAbxu2ynYZNhINBBeB7SfBEGS5tqBX+BogaapGkCvIbwroWVrpoFdKECE3XQF910rL9erpyraNrQQDkKEmyKLY22RpvqG1vZaX16scI6HGBJRyh77VGcq0LtS1FPw6tvYrrcQS8kytG3CF87uMDaoELmUzE2k71JNK5CtYZSAn1UkhhbhhFC52LQSIrp6Ho5FU0jVdRWggTSUuRajmA1pA5RR7qkVh2iq9zp/y1KirqkatEJEw6LI6kg0SoSj8ZQpR+MimTYtXZFSGTK6vJ5kApDlZZ7lySAnMZLESbmNstFyNI/bqc40FM0hzfa2mmgRxFnD8dywedxgpzajzYqoyMko3y+Y45APsmucomarmEJxw6krTGAivO7jtSsw+jZClialEByNMH7BpShEiMmkphDjF10t5uTTaYeIOEJJwedLp9NUmqVMa72PAYD2rVraEpPjMCES2Nfp9ay/du0XSM2FImNtYX8BZZI4l06sVZyAsZ4I4x7284Ec72PTCo+3/suQh9k3tiMmqkNkMSjjGRNgFBb4eV6eiA4J50Tqc/KAkpghE6K1EaKkLsqQlLHOUgloaYrA8irDBlRIKjVBleSCqkpKvFJD0iqEAEJJkoOB/1OjXK/UY7KZhFFT1+TMhAh+wsTOWkpUtFAmBnUdG65X9VcEaTsgbzs8p9dvCKITw8ZBxKRGOdqmZDPhM0U81BzTOjfrW8Kt4fPwrioqBphFqinZg4xy4VL2JpmyoG2mLHyGU23OXG83N0IDdwmyTF2H1kr6lpiYuIl+h6b5FVHJuoZpXHe3IbvBMXmT2hbRHURd3OPpuAJymge4qlzAf2tqrXPr2p75D4bWDRW20bQRVG7DB4hv7HUoXOD+6B7Px6DH8wG+UQEfmENXgcrSohXFRWUVtoYgpYkqZWvrDfyVb0FqI8wkRc0qLPV0zBroX5d3AetbC2aMXsEmF9FT8u5jYNbllRL6vunltB8EaQ74OZqm14Cqy6vF9IPF08oemfVoWlnV/9zfp56Jb7t/2+9/8IOgfNTJ4ykpwMooONbwUs3M6a38yMDJtx+rq5SmlZ3Z17ug5bfBc7v3jJxhv18y79Ck9+a9cbbs8EDz8Fr9+TeHRuYfr336kyWf9aiVW6u/014c3v1rsLfw1Fx+YeHhrpnw02DVO5tbd/55Ysve+tiiZct2WcZbJU/81HXvA+rPof3NM/oeenjf4Ef97efC2/U9+0bK2YXxAx21p4deLZ2XPliVOHvh/Q3qzl3bza9+/Lx6xwWmujTR8eyOv3zD06xNXUdjP+zS7zm4YviF9JeNRQfOD84+8uHeoddnt758RGrZ8u6TA8e/qGCPvbL/4tRDFd7j2+K1g1+f/Hbh/Dmd/emnXlt7cfPWqmYVrN4xMrRgdd2JrsrTR6lvsuX7B4CTuGQaDwAA",
                CertID = "PRD-8e35c5356f0f-1449-425a-a638-6322"
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.AppID", "App ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.CertID", "Cert ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.Token", "Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.DevID", "Dev ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.CategoryId", "Category Worldbuy");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.CategoryEbayId", "Category Ebay");
            this.AddOrUpdatePluginLocaleResource("Plugins.AffiliateEbay.MapCategory.Error", "Danh mục đã được map");
            this.AddOrUpdatePluginLocaleResource("Plugins.AffiliateEbay.MapCategory.Added", "Map danh mục thành công");
            this.AddOrUpdatePluginLocaleResource("Plugins.AffiliateEbay.CallApi.Error", "Map danh mục trước khi lấy sản phẩm");
            this.AddOrUpdatePluginLocaleResource("Admin.Catalog.Products.Added", "Lấy sản phẩm thành công");
            this.AddOrUpdatePluginLocaleResource("Plugin.Configuration.Settings.Ebay", "Lấy sản phẩm thành công");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Ebay.Mapping", "Mapping Category");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Ebay.Product", "Get Product");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Ebay.Setting", "Config Plugin");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Affiliate.Ebay", "Plugin Ebay");
            this.AddOrUpdatePluginLocaleResource("Plugin.Configuration.Settings.Ebay", "Configure Ebay");
            this.AddOrUpdatePluginLocaleResource("Plugin.Ebay.Configuration.Settings.CallApi", "Get Product Ebay");
            this.AddOrUpdatePluginLocaleResource("Plugin.Ebay.Configuration.Settings.MappingCategory", "Mapping Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.KeyWord", "Keyword");

            //Insert CategoryEbay   
            // set devId, appId, certId in ApiAccount
            ApiAccount account = new ApiAccount();
            account.Developer = "4343ce44-efa1-4c33-a431-18daed74c054";
            account.Application = "PhuocPha-worldbuy-PRD-28e35c535-94b2bb4b";
            account.Certificate = "PRD-8e35c5356f0f-1449-425a-a638-6322";

            // set ApiAccount and token in ApiCredential
            ApiCredential credential = new ApiCredential();
            credential.ApiAccount = account;
            credential.eBayToken = "AgAAAA**AQAAAA**aAAAAA**JYLgWQ**nY+sHZ2PrBmdj6wVnY+sEZ2PrA2dj6ACl4CiC5KHowqdj6x9nY+seQ**Pt8DAA**AAMAAA**XEESS+BUaGiTZWWj64HwUby9/+ABWbn3NBturqmQgtWJzIW3f/FNc8Wm/EtypFpT5NU6yfvf9lBvyp7kUifGyih/PIQTmDWVrDJjXU+PxJX6QEGFzw4noZK/E1n01EFm6heicUJ5XVj8KY0GA7WVatXy3k2AVDc6psRDoscGAya5mZVDhHgqdyiTHTs/G/5y6G01HBS9ehbkui4eRbggCX6eyee9nCz/UmLOn19C9WJmCpLng8PxcifyEu4Ca8UInQJsaAqYKKGVflBofV0D+6jd4URuEaKl3J2lZbUrjPIBgmCZtYHv8LOKw5OgCRQKCl014DBz91PgOKpUfZCvh7suXazzNOdjh2t4qitzPCi7uMTmzXUzN7WjN5pbFx+3n90FOe+uKjZN1ZhrFx7295qtt5eFh7xxV0ZqdwbBQUILUKm9tE5/KYlpKS+3wjMFpTw66hYuw0s2CIv7axomq+NV2d+rEY2FXAKD3v8OWVnbIFjzQJY+cV8bpMxxDkHsTN9G7lOhbCGxGlRkV1xy5JYeGmqzGYhuDRZ9glfERySkCXJflU94H/bOls8GgEVDrV6iDvcBgPYVhf8YgLN4MUCY3U/osTZbAXh+bV5RPgvtZLNQ4ZB1WhP/qtCsufdkJKnnpxYy1Zswl8QBRZK4+TVkf7/CEqfNSsJGEllxzmznD+Ox4KL+A1n8nzk/NYPMceXFZPS3l4047juy6lTQ6BSBzP+N0CMjezHAoA4tE0vdlJKTRFPyt1sSCjj03MVX";

            // add ApiCredential to ApiContext
            ApiContext context = new ApiContext();
            context.ApiCredential = credential;

            // set eBay server URL to call
            context.SoapApiServerUrl = "https://api.ebay.com/wsapi";

            // set timeout in milliseconds - 3 minutes
            context.Timeout = 180000;

            // set wsdl version number
            context.Version = "1027";

            // create ApiCall object - we'll use it to make the call
            GetCategoriesCall apicall = new GetCategoriesCall(context);
            apicall.LevelLimit = 3;
            apicall.DetailLevelList.Add(DetailLevelCodeType.ReturnAll);

            CategoryTypeCollection cats = apicall.GetCategories();

            foreach (CategoryType category in cats)
            {
                var cate = new CategoryEbay();
                var ebayId = Convert.ToInt32(category.CategoryParentID.ItemAt(0));
                var getCate = _affiliateEbayService.GetByEbayId(ebayId);
                if (getCate != null)
                    cate.ParentCategoryId = getCate.Id;
                else
                    cate.ParentCategoryId = 0;
                cate.EbayId = Convert.ToInt32(category.CategoryID);
                cate.Name = category.CategoryName;
                cate.Level = category.CategoryLevel;
                cate.Published = true;
                cate.Deleted = false;
                _affiliateEbayService.InsertCategoryEbay(cate);
            }

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _context.Uninstall();

            //settings
            _settingService.DeleteSetting<ConfigurationModel>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.AppID");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.CertID");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.Token");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.DevID");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.CategoryId");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.CategoryEbayId");
            this.DeletePluginLocaleResource("Plugins.AffiliateEbay.MapCategory.Error");
            this.DeletePluginLocaleResource("Plugins.AffiliateEbay.MapCategory.Added");
            this.DeletePluginLocaleResource("Plugins.AffiliateEbay.CallApi.Error");
            this.DeletePluginLocaleResource("Admin.Catalog.Products.Added");
            this.DeletePluginLocaleResource("Plugin.Configuration.Settings.Ebay");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Ebay.Mapping");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Ebay.Product");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Ebay.Setting");
            this.DeletePluginLocaleResource("Nop.Plugin.Affiliate.Ebay");
            this.DeletePluginLocaleResource("Plugin.Configuration.Settings.Ebay");
            this.DeletePluginLocaleResource("Plugin.Ebay.Configuration.Settings.CallApi");
            this.DeletePluginLocaleResource("Plugin.Ebay.Configuration.Settings.MappingCategory");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.Ebay.KeyWord");

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

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Affiliate.Ebay");
            if (pluginNode == null)
            {
                pluginNode = new SiteMapNode()
                {
                    SystemName = "Affiliate.Ebay",
                    Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Ebay"),
                    Visible = true,
                    IconClass = "fa-dot-circle-o",
                    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                };
                groupNode.ChildNodes.Add(pluginNode);

            }
            var configItem = new SiteMapNode()
            {
                SystemName = "Affiliate.Ebay.Setting",
                Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Ebay.Setting"),
                ControllerName = "AffiliateEbay",
                ActionName = "Configure",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(configItem);

            var getProductItem = new SiteMapNode()
            {
                SystemName = "Affiliate.Ebay.Product",
                Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Ebay.Product"),
                ControllerName = "AffiliateEbay",
                ActionName = "CallApi",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(getProductItem);

            var mappingItem = new SiteMapNode()
            {
                SystemName = "Affiliate.Ebay.Mapping",
                Title = _localizationService.GetResource("Nop.Plugin.Affiliate.Ebay.Mapping"),
                ControllerName = "AffiliateEbay",
                ActionName = "MapCategory",
                Visible = true,
                IconClass = "fa-dot-circle-o",
                RouteValues = new RouteValueDictionary() { { "Area", "Admin" } },
            };

            pluginNode.ChildNodes.Add(mappingItem);
        }

        #endregion
    }
}
