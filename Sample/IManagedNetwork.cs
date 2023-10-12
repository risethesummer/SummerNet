using Autofac;

public interface IManagedNetwork : IDisposable
{
    public ILifetimeScope? Scope { set; get; }
}