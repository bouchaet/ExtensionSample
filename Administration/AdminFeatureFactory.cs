using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            // listener.OutPort.OnDataSentAsync +=
            //     device => parser.InputPort.Receive(device.ReadLine());

            // Another approach. Request/Response
            // adminserver.Start() performed on listener by the feature
            var adminserver = new AdministrationServer(listener);
            adminserver.Add("*", s => DefaultRoute(s, parser));
            adminserver.Add("help", s => HelpRoute(parser));

            parser.OutputPort.Connect(controller.InputCommandPort);

            var cmds = BuildCommands(commandCollection, presenter); // todo : presenter holds a view which isn't thread safe
            var feature = new BasicAdminFeature(commandCollection, listener);

            foreach (var cmd in cmds)
                feature.AddCommand(cmd);

            return feature;
        }

        private static string HelpRoute(CommandParser parser)
        {
            parser.InputPort.Receive("admin list");
            return string.Empty;
        }

        private static string DefaultRoute(
            string input,
            FeatureCommandCollection commandCollection,
            IContainerBuilder components)
        {
            Logger.WriteInfo("Running '*' route.");
            var cmdparser = new CommandParser(commandCollection,
                components.Get<Port<string>>(),
                new CommandPort()
            );

            var port = new CommandPort();
            cmdparser.OutputPort.Connect(port);
            port.OnDataReceived += (s, cmd) => cmd.Execute();

            cmdparser.InputPort.Receive(input);

            return string.Empty;
        }

        private static string DefaultRoute(string input,
            CommandParser parser)
        {
            Logger.WriteInfo("Running '*' route using command parser.");
            parser.InputPort.Receive(input);
            return string.Empty;
        }

        private static IEnumerable<ICommand> BuildCommands(
            FeatureCommandCollection coll,
            ICommandPresenter presenter)
        {
            var listCommand = new ListCommand(coll, new StringListPort());
            listCommand.OutPort.OnDataSent +=
                (sender, list) => presenter.ShowCommands(list);

            yield return listCommand;
            yield return new HistoryCommand();
        }
    }
}