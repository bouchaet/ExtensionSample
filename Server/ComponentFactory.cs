using System.Collections.Generic;
using Aws.Database.DynamoDb;
using BfmEvent.Details;
using Entities;
using Entities.Http;
using JournalEntry.Adapters;
using JournalEntry.Details;
using JournalEntry.UseCases;
using Server.Adapters;
using Server.Details;
using Server.UseCases;
using NonPersistentQueueManager;
using Server.Details.Dependency;
using Server.Details.Devices;
using Server.Details.Ports;
using Server.Adapters.Http;

namespace Server
{
    internal static class ComponentFactory
    {
        public static IContainerBuilder CreateComponents()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Set<IListener<IDevice>>().To<SocketListener>();
            containerBuilder.Set<IDynamicLoader>().To<DynamicLoader>();
            //containerBuilder.Set<IDevice>().To<ConsoleDevice>();
            containerBuilder.Set<IDevice>().To<TcpDevice>(new SingletonLifeManager());
            containerBuilder.Set<Port<string>>().To<StringPort>();
            containerBuilder.Set<IMapper<JournalEntry.UseCases.JournalEntry, PartnerGlEntry>>()
                .To<Mapper>();
            //containerBuilder.Set<IArchiver<Entities.BfmEvent>>().To<InMemoryArchiver>();
            containerBuilder.Set<IArchiver<Entities.BfmEvent>>().To<DynamoDbArchiver>();
            containerBuilder.Set<Port<Entities.BfmEvent>>().To<BfmEventPort>();
            containerBuilder.Set<IDeserializer<Entities.BfmEvent>>().To<TokenDeserializer>();
            containerBuilder.Set<IInfoServicesGateway>().To<InMemoryInfoSvcGateway>();
            containerBuilder.Set<IPluginManager>().To<PluginManager>(
                containerBuilder.Get<IDynamicLoader>());

            containerBuilder.Set<IPresenter<PartnerGlEntry>>().To<GlEntryPresenter>(
                new NewtonSerializer(),
                new CsvSerializer()
            );

            containerBuilder.Set<UseCases.Server>().To<FeatureServer>(containerBuilder);

            containerBuilder.Set<IAccumulator>().To<Accumulator>(
                containerBuilder.Get<IArchiver<Entities.BfmEvent>>(),
                new SizeByDatePolicy(2)
            );

            containerBuilder.Set<IQueueMessageFactory>().To<BasicQueueMessageFactory>();
            containerBuilder.Set<IQueueFactory>().To<QueueFactory>(
                containerBuilder.Get<IQueueMessageFactory>()
            );
            containerBuilder.Set<IQueueManager>().To<VolatileQueueManager>(
                new SingletonLifeManager(),
                containerBuilder.Get<IQueueFactory>(),
                new Dictionary<string, IQueue>()
            );

            containerBuilder.Set<HttpServer>().To<TcpSocketHttpServer>();

            return containerBuilder;
        }
    }
}