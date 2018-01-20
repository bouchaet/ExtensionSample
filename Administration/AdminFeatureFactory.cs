using System.Collections.Generic;
using Administration.Adapters;
using Administration.Details;
using Administration.UseCases;
using Entities;

namespace Administration
{
    public static class AdminFeatureFactory
    {
        internal class PortDeviceProvider : IDeviceProvider
        {
            private IDevice _device;

            public PortDeviceProvider(Port<IDevice> port)
            {
                port.OnDataSent += (sender, device) => _device = device;
            }

            public IDevice GetDevice()
            {
                return _device;
            }
        }

        public static AdminFeature Create(IContainerBuilder components)
        {
            var listener = components.Get<IListener<IDevice>>();
            var deviceProvider = new PortDeviceProvider(listener.OutPort); // todo: not threadsafe
            var view = new DeviceCommandView(deviceProvider);

            var presenter = new CommandPresenter();
            presenter.AttachView(view);

            var commandCollection = new FeatureCommandCollection();

            var parser = new CommandParser(commandCollection,
                components.Get<Port<string>>(),
                new CommandPort()
            );

            var controller = new Controller(new CommandPort());
            //listener.OutPort.Connect(parser.InputPort);

            listener.OutPort.OnDataSentAsync +=
                device => parser.InputPort.Receive(device.ReadLine());

            listener.OutPort.OnDataSentAsync +=
                device => Logger.WriteInfo($"ADMIN: received {device.ReadLine()}");

            parser.OutputPort.Connect(controller.InputCommandPort);

            var cmds = BuildCommands(commandCollection, presenter); // todo : presenter holds a view which isn't thread safe
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