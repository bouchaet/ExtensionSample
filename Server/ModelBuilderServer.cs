using Entities;
using ModelBuilder;
using ModelBuilder.Adapters;
using Server.Adapters;

namespace Server
{
    sealed class ModelBuilderServer : IServer
    {
        private readonly IContainerBuilder _components;

        internal ModelBuilderServer(IContainerBuilder components)
        {
            _components = components;
        }

        public void Start()
        {
            var pluginMgr = new PluginManager(_components.Get<IDynamicLoader>());
            var modelBuilderMgr = new ModelBuilderManager
            {
                AdditionalBuildersFunc = () => pluginMgr.Load<IModelBuilder>()
            };

            var controller = new ModelController(
                modelBuilderMgr,
                _components.Get<Port<string>>(),
                _components.Get<Port<string>>()
            );

            var modelListener = new ModelListener(
                _components.Get<IDevice>(),
                controller
            );

            modelListener.Listen();
        }
    }
}