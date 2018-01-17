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
using Nop.Plugin.Worldbuy.SimpleMenu.Data;
using Nop.Plugin.Worldbuy.SimpleMenu.Domain;
using Nop.Plugin.Worldbuy.SimpleMenu.Services;

namespace Nop.Plugin.Worldbuy.SimpleMenu
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_wb_simple_menu";
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<WB_SimpleMenuService>().As<IWB_SimpleMenuService>().InstancePerLifetimeScope();
            builder.RegisterType<WB_SimpleMenuItemService>().As<IWB_SimpleMenuItemService>().InstancePerLifetimeScope();
            //data context
            this.RegisterPluginDataContext<SimpleMenuObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<WB_SimpleMenu>>()
                .As<IRepository<WB_SimpleMenu>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<WB_SimpleMenuItem>>()
               .As<IRepository<WB_SimpleMenuItem>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();
        }
    }
}
