using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace TravelRepublic.Flights.Web
{
    public class WindsorInstaller : IWindsorInstaller
    {
        private readonly string assemblyRootPath;

        public WindsorInstaller(string assemblyRootPath="bin")
        {
            this.assemblyRootPath = assemblyRootPath;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Note: Using convention based component registration
            // http://docs.castleproject.org/Windsor.Registering-components-by-conventions.ashx

            var trpAssemblies = new AssemblyFilter(this.assemblyRootPath).FilterByName(an => an.Name.StartsWith("TravelRepublic.Flight"));

            container.Register(
                Classes.FromAssemblyInDirectory(trpAssemblies)
                       .BasedOn<IHttpController>()
                       .Configure(c => c.LifestyleTransient()),

                Classes.FromAssemblyInDirectory(trpAssemblies)
                       .Pick()
                       .WithService.DefaultInterfaces()
                );
        }
    }
}