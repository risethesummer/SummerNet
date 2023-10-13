using Autofac;

namespace Realtime.Objects;

public interface IDependenciesInstaller : IDisposable
{
    ILifetimeScope? Install<T>(in T injectable, ILifetimeScope scope) where T : IManagedNetwork;
}