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
using Nop.Plugin.Worldbuy.AnyBanner.Data;
using Nop.Plugin.Worldbuy.AnyBanner.Domain;
using Nop.Plugin.Worldbuy.AnyBanner.Services;

namespace Nop.Plugin.Worldbuy.AnyBanner
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_wb_any_banner";
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WB_AnyBannerService>().As<IWB_AnyBannerService>().InstancePerLifetimeScope();
            builder.RegisterType<WB_AnyBannerItemService>().As<IWB_AnyBannerItemService>().InstancePerLifetimeScope();
            //builder.RegisterType<StateProvinceWBImportManager>().As<IStateProvinceWBImportManager>().InstancePerLifetimeScope();
            //data context
            this.RegisterPluginDataContext<AnyBannerObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<WB_AnyBanner>>()
                .As<IRepository<WB_AnyBanner>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<WB_AnyBannerItem>>()
               .As<IRepository<WB_AnyBannerItem>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();
        }
    }
}
