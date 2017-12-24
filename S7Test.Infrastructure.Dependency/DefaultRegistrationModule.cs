using Autofac;
using S7Test.Core.Interface.Common;
using S7Test.Core.Interface.Service.App;
using S7Test.Core.Interface.Service.Domain;
using S7Test.Core.Service;
using S7Test.Infrastructure.Data;
using S7Test.Infrastructure.Data.Common;
using S7Test.Service;

namespace S7Test.Infrastructure.Dependency
{
    public class DefaultRegistrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppDbContext>().InstancePerLifetimeScope().As<IDataContext>();
            builder.RegisterType<UnitOfWork>().InstancePerLifetimeScope().As<IUnitOfWork>();
            //Domain services
            builder.RegisterType<PlayerService>().InstancePerLifetimeScope().As<IPlayerService>();

            //app services
            builder.RegisterType<AuthMessageSender>().InstancePerLifetimeScope().As<IEmailSender>();
            builder.RegisterType<AuthMessageSender>().InstancePerLifetimeScope().As<ISmsSender>();

        }
    }
}
