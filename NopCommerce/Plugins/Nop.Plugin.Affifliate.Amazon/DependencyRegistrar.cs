using Nop.Core.Infrastructure.DependencyManagement;
using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Mvc;
using Autofac.Core;
using Nop.Data;
using Nop.Core.Data;
using Nop.Plugin.Affiliate.Amazon.Services;
using Nop.Plugin.Affiliate.Amazon.Data;
using Nop.Core.Caching;
using Nop.Plugin.Affiliate.Amazon.Domain;

namespace Nop.Plugin.Affiliate.Amazon
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_affiliate_amazon";
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            this.RegisterPluginDataContext<AmazonObjectContext>(builder, CONTEXT_NAME);

            builder.RegisterType<AmazonProvider>().As<IAmazonProvider>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope();

            builder.RegisterType<CategoryAmazonService>().As<ICategoryAmazonService>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliateAmazonImportManager>().As<IAffiliateAmazonImportManager>().InstancePerLifetimeScope();
            builder.RegisterType<AmazonService>().As<IAmazonService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAmazonService>().As<IProductAmazonService>().InstancePerLifetimeScope();

            //data context
            

            //override required repository with our custom context
            builder.RegisterType<EfRepository<CategoryAmazon>>()
                .As<IRepository<CategoryAmazon>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }
    }
}
