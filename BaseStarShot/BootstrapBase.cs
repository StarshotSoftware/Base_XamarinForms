using Base1902;
using Base1902.Dependencies;
using BaseStarShot.Logging;
using BaseStarShot.Repositories;
using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("BaseStarShot.Droid")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("BaseStarShot.iOS")]
namespace BaseStarShot
{
    public class BootstrapBase
    {
        public static void Init()
        {
            RegisterDependencies();
        }

        public static void Init(IDependencyResolver resolver)
        {
            RegisterDependencies(resolver);
        }

        public static void RegisterDependencies()
        {
            RegisterDependencies(Resolver.Current);
        }

        public static void RegisterDependencies(IDependencyResolver resolver)
        {
            resolver.Register<IDispatcherService, DispatcherService>();
            resolver.Register<IDeviceService, DeviceService>();
            resolver.Register<IErrorRepository, ErrorRepository<ErrorData>>();
            resolver.Register<IErrorService, ErrorService>();
            resolver.Register<IPlatformService, DefaultPlatformService>();
            resolver.Register<ILastInteractionListener, LastInteractionListener>();
            resolver.Register<Net.IHttpClientManager, Net.HttpClientManager>();
        }
    }
}
