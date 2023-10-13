using Autofac;

namespace Realtime.Objects;

public interface IManagedNetwork : IDisposable
{
    public ILifetimeScope? Scope { set; get; }
}