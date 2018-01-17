using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Order.Amazon.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Order.Amazon.Service")]
        public string Service { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Order.Amazon.AssociateTag")]
        public string AssociateTag { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Order.Amazon.AWSAccessKeyID")]
        public string AWSAccessKeyID { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Order.Amazon.AWSSecretKey")]
        public string AWSSecretKey { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Order.Amazon.Endpoint")]
        public string Endpoint { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Order.Amazon.Version")]
        public string Version { get; set; }
    }
}