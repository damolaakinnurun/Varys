using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Varys.Elastic;
using Varys.Elastic.Services;
using Autofac.Integration.WebApi;

namespace Varys.API.Infrastructure
{
    public class FormDependencyResolver
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //Registering mvc controllers. this registers all the controllers at once
            builder.RegisterControllers(typeof(WebApiApplication).Assembly);

            if (HttpContext.Current != null)
            {
                builder.Register(x =>
               (new HttpContextWrapper(HttpContext.Current) as HttpContextBase))
               .As<HttpContextBase>()
               .InstancePerRequest();
            }
            //builder.RegisterType<ManifestService>().As<IManifestService>().InstancePerRequest();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerRequest();

            //setting the dependency resolver to be autofac
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
          
        }
    }
}