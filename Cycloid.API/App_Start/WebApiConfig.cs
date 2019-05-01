using System.Net.Http.Formatting;
using System.Web.Http;
using Cycloid.Managers;
using Cycloid.Repositories;
using Cycloid.Services;
using Unity;

namespace Cycloid.API
{
    /// <summary>
    /// The web api config class
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Register configuration settings
        /// </summary>
        /// <param name="config">The configuration</param>
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());

            var container = new UnityContainer();

            container.RegisterType<IChannelsManager, ChannelsManager>(TypeLifetime.Scoped);
            container.RegisterType<IProgramsManager, ProgramsManager>(TypeLifetime.Scoped);
            container.RegisterType<IChannelsService, ChannelsWcfService>(TypeLifetime.Singleton);
            container.RegisterType<IDevicesRepository, DevicesRepository>(TypeLifetime.Singleton);
            container.RegisterType<IProgramsService, ProgramsRestService>(TypeLifetime.Singleton);

            config.DependencyResolver = new UnityResolver(container);
            
            config.MessageHandlers.Add(new AuthenticationHandler());
        }
    }
}
