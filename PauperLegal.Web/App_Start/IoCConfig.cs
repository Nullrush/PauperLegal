using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace PauperLegal.Web
{
    public class IoCConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes()
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();

            builder.RegisterControllers(typeof (MvcApplication).Assembly);

            builder.RegisterAssemblyModules(typeof(MvcApplication).Assembly);

            builder.RegisterModule<AutofacWebTypesModule>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}