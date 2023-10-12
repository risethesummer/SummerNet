using Autofac;

namespace Realtime.Controllers.Objects;

public interface IManagedNetwork : IDisposable
{
    public ILifetimeScope? Scope { set; get; }
}