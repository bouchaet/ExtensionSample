using BfmEvent;
using BfmEvent.Details;
using Entities;
using JournalEntry.Adapters;
using JournalEntry.Details;
using JournalEntry.UseCases;
using Server.Adapters;
using Server.Details;
using BfmEventDS = Entities.BfmEvent;
using JournalEntryDS = JournalEntry.UseCases.JournalEntry;

namespace Server
{
    internal static class Program
    {
        private static void Main()
        {
            var components = CreateComponents();
            components.Get<Server>().Start();
        }

        private static IContainerBuilder CreateComponents()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Set<IDynamicLoader>().To<DynamicLoader>();
            containerBuilder.Set<IDevice>().To<ConsoleDevice>();
            containerBuilder.Set<Port<string>>().To<StringPort>();
            containerBuilder.Set<IMapper<JournalEntryDS, GlEntry>>().To<Mapper>();
            containerBuilder.Set<IArchiver<BfmEventDS>>().To<InMemoryArchiver>();
            containerBuilder.Set<Port<BfmEventDS>>().To<BfmEventPort>();
            containerBuilder.Set<IDeserializer<BfmEventDS>>().To<TokenDeserializer>();
            containerBuilder.Set<IInfoServicesGateway>().To<InMemoryInfoSvcGateway>();

            containerBuilder.Set<IPresenter<GlEntry>>().To<GlEntryPresenter>(
                new NewtonSerializer(),
                new CsvSerializer()
            );

            containerBuilder.Set<Server>().To<FeatureServer>(containerBuilder);

            containerBuilder.Set<IAccumulator>().To<Accumulator>(
                containerBuilder.Get<IArchiver<BfmEventDS>>(),
                new SizeByDatePolicy(2)
            );

            return containerBuilder;
        }
    }
}