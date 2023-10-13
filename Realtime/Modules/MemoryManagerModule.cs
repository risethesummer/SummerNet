using Autofac;
using Realtime.Transporters.Messages;
using Realtime.Utils.Buffers;
using Realtime.Utils.Factory;

namespace Realtime.Modules;

/// <summary>
/// Pool for reusable UnmanagedMemoryManager
/// </summary>
public class MemoryManagerModule : Module
{
    protected override void Load(ContainerBuilder moduleBuilder)
    {
        moduleBuilder.RegisterType<UnmanagedMemoryManager<byte>>()
            .AsSelf().InstancePerDependency().OwnedByLifetimeScope(); // Call dispose for all UnmanagedMemoryManager<byte>
        moduleBuilder.RegisterType<MessageEncoder>()
            .As<IMessageEncoder>().SingleInstance();
        moduleBuilder.RegisterType<MessageDecoder>()
            .As<IMessageDecoder>().SingleInstance();
        moduleBuilder.RegisterType<ParallelBuffer<RawReceivedNetworkMessage<int>>>()
            .InstancePerDependency().OwnedByLifetimeScope();
        moduleBuilder.RegisterType<ParallelBuffer<DecodedSentMessage<int>>>()
            .InstancePerDependency().OwnedByLifetimeScope();
        moduleBuilder
            .RegisterType<Factory<BufferPointer<byte>, 
                PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>>()
            .As<IFactory<BufferPointer<byte>, 
                PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>>()
            .InstancePerDependency();
        moduleBuilder
            .RegisterDecorator<PoolFactory<BufferPointer<byte>, 
                    PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>, 
                IFactory<BufferPointer<byte>, 
                    PoolableWrapper<BufferPointer<byte>, UnmanagedMemoryManager<byte>>>>();
            
    }
}