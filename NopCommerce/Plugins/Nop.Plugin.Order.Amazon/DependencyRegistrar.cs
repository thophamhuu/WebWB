using Nop.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Mvc;
using Autofac.Core;
using Nop.Data;
using Nop.Core.Data;
using Nop.Plugin.Order.Amazon.Data;
using Nop.Plugin.Order.Amazon.Domain;
using Nop.Plugin.Order.Amazon.Service;

namespace Nop.Plugin.Order.Amazon
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_affiliate_amazon_order";
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<OrderAmazonService>().As<IOrderAmazonService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<OrderAmazonObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<OrderAmazon>>()
                .As<IRepository<OrderAmazon>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }
    }
}
