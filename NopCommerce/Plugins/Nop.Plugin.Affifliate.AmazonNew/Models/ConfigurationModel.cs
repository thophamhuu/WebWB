using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Affiliate.Amazon.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Service")]
        public string Service { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Endpoint")]
        public string Endpoint { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Version")]
        public string Version { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.Amazon.Durations")]
        public int Durations { get; set; }
    }
}