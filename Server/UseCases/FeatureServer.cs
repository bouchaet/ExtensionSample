using System.Linq;
using Administration;
using Entities;
using JournalEntry;
using ModelBuilder;
using PublishingFacade;

namespace Server.UseCases
{
    internal class FeatureServer : Server
    {
        public FeatureServer(IContainerBuilder components)
            : base(components)
        {
        }

        private static IFeature CreateFeature(IContainerBuilder components)
        {
            var mainFeature = AdminFeatureFactory.Create(components);
            var features = new IFeature[]
            {
                mainFeature,
                JournalEntryFeatureFactory.Create(components),
                ModelBuilderFeatureFactory.Create(components),
                PublishingFacadeFeatureFactory.CreateFeature(components)
            };

            mainFeature.SetFeatureCommandCollection(
                features.ToDictionary(f => f.Name, f => f.Commands)
            );

            return mainFeature;
        }

        protected override ISwitchable BuildSwitch(IContainerBuilder components)
        {
            var feature = CreateFeature(components);
            var featureSwitch = new FeatureSwitch(feature);
            return featureSwitch;
        }
    }
}