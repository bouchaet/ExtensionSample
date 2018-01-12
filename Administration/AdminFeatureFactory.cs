using System.Collections.Generic;
using Administration.Adapters;
using Administration.Details;
using Administration.UseCases;
using Entities;

namespace Administration
{
    public static class AdminFeatureFactory
    {
        public static AdminFeature Create(IContainerBuilder components)
        {
            var device = components.Get<IDevice>();
            var view = new DeviceCommandView(device);

            var presenter = new CommandPresenter();
            presenter.AttachView(view);

            var commandCollection = new FeatureCommandCollection();

            var parser = new CommandParser(commandCollection,
                components.Get<Port<string>>(),
                new CommandPort()
            );

            var listener = new StringDeviceListener(device, components.Get<Port<string>>());

            var controller = new Controller(new CommandPort());

            listener.OutPort.Connect(parser.InputPort);
            parser.OutputPort.Connect(controller.InputCommandPort);

            var cmds = BuildCommands(commandCollection, presenter);
            var feature = new BasicAdminFeature(commandCollection, listener);

            foreach (var cmd in cmds)
                feature.AddCommand(cmd);

            return feature;
        }

        private static IEnumerable<ICommand> BuildCommands(FeatureCommandCollection coll,
            ICommandPresenter presenter)
        {
            var listCommand = new ListCommand(coll, new StringListPort());
            listCommand.OutPort.OnDataSent += (sender, list) => presenter.ShowCommands(list);

            yield return listCommand;
            yield return new HistoryCommand();
        }
    }
}