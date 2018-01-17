
using Nop.Core.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nop.Plugin.Order.Amazon
{
    public class OrderAmazonSettings : ISettings
    {
        public DateTime? RunTaskTime { get; set; }
        public string Service { get; set; }

        public string Endpoint { get; set; }
        public string Version { get; set; }
        public string Accounts { get; set; }
        public string AssociateTag { get; set; }
        public string AWSAccessKeyID { get; set; }
        public string AWSSecretKey { get; set; }
    }
}