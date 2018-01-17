using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Affiliate.Ebay.Data;
using Nop.Plugin.Affiliate.Ebay.Domain;
using Nop.Plugin.Affiliate.Ebay.Services;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<AffiliateEbayService>().As<IAffiliateEbayService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<CategoryEbayObjectContext>(builder, "nop_object_context_affiliate_ebay");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<CategoryEbay>>()
                .As<IRepository<CategoryEbay>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_affiliate_ebay"))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}
