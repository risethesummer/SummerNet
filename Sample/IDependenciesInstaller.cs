using Autofac;

public interface IDependenciesInstaller : IDisposable
{
    ILifetimeScope? Install<T>(in T injectable, ILifetimeScope scope) where T : IManagedNetwork;
}