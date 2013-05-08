using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Castle.Windsor;
using TravelRepublic.IoC;
using TravelRepublic.WebApi;

namespace TravelRepublic.Flights.Web
{
    public class WebApiSetup : IDisposable
    {
        private readonly IWindsorContainer container;

        public WebApiSetup(HttpConfiguration config, string assemblyRootPath)
        {
            this.container = new WindsorContainer().Install(new WindsorInstaller(assemblyRootPath));

            config.Services.Replace(
                typeof(IHttpControllerActivator),
                new WindsorWebApiCompositionRoot(this.container));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional }
                );

            // Easy CORS support requires the Microsoft.AspNet.WebApi.Cors package which requires .NET 4.5
            // http://weblogs.asp.net/scottgu/archive/2013/04/19/asp-net-web-api-cors-support-and-attribute-based-routing-improvements.aspx
            // http://blogs.msdn.com/b/yaohuang1/archive/2013/04/05/try-out-asp.net-web-api-cors-support-using-the-nightly-builds.aspx
            // http://aspnetwebstack.codeplex.com/wikipage?title=CORS%20support%20for%20ASP.NET%20Web%20API
            // config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // Jsonp support
            // http://www.west-wind.com/weblog/posts/2012/Apr/02/Creating-a-JSONP-Formatter-for-ASPNET-Web-API
            config.Formatters.Insert(0, new JsonpFormatter());
        }

        public void Dispose()
        {
            this.container.Dispose();
        }
    }
}