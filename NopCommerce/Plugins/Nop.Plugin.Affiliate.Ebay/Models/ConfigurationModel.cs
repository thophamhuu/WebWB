using Nop.Core.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Affiliate.Ebay.Models
{
    public class ConfigurationModel : BaseNopModel, ISettings
    {
        public ConfigurationModel()
        {
            AvailableCategory = new List<SelectListItem>();
            AvailableCategoryEbay = new List<SelectListItem>();
        }
        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Plugins.Widgets.Affiliate.Ebay.CertID")]
        public string CertID  { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.Ebay.AppID")]
        public string AppID { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.Ebay.DevID")]
        public string DevID { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.Ebay.Token")]
        public string Token { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.Ebay.CategoryId")]
        public int CategoryId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.Ebay.CategoryEbayId")]
        public int CategoryEbayId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.Ebay.KeyWord")]
        public string KeyWord { get; set; }

        public List<SelectListItem> AvailableCategory { get; set; }
        public List<SelectListItem> AvailableCategoryEbay { get; set; }
    }
}
