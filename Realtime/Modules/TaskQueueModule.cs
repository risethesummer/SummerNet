using Autofac;
using Realtime.Utils.Factory;

namespace Realtime.Modules;

public class TaskQueueModule : Module
{
    protected override void Load(ContainerBuilder moduleBuilder)
    {
        moduleBuilder.RegisterType<Factory<PoolableWrapper<DisposableQueue<Task>>>>()
            .As<IFactory<PoolableWrapper<DisposableQueue<Task>>>>()
            .InstancePerDependency();
        moduleBuilder.RegisterDecorator<PoolFactory<PoolableWrapper<DisposableQueue<Task>>>, 
            IFactory<PoolableWrapper<DisposableQueue<Task>>>>();
    }
}