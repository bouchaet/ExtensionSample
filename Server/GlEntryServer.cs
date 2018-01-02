using System;
using System.Collections.Generic;
using System.IO;
using BfmEvent;
using Entities;
using JournalEntry.Adapters;
using JournalEntry.Details;
using JournalEntry.UseCases;
using Server.Adapters;
using BfmEventDS = Entities.BfmEvent;
using JournalEntryDS = JournalEntry.UseCases.JournalEntry;

namespace Server
{
    sealed class GlEntryServer : IServer
    {
        private readonly IContainerBuilder _components;

        public GlEntryServer(IContainerBuilder components)
        {
            _components = components;
        }

        public void Start()
        {
            StartGlEntry(_components);
        }

        private static void StartGlEntry(IContainerBuilder components)
        {
            var presenter = CreatePresenterPort(components);
            var controller = CreateController(components, presenter);
            var listener = CreateListener(components);

            listener.OutPort.Connect(controller.EventPort);
            listener.Listen();
        }

        private static Port<IEnumerable<GlEntry>> CreatePresenterPort(IContainerBuilder components)
        {
            var presenter = components.Get<IPresenter<GlEntry>>();
            var csvView = new CsvView(Path.Combine(Environment.CurrentDirectory, "glentries.csv"));
            var jsonView =
                new JsonView(Path.Combine(Environment.CurrentDirectory, "glentries.json"));
            presenter.AttachView(csvView);
            presenter.AttachView(jsonView);

            return presenter as Port<IEnumerable<GlEntry>>; //todo: eurk
        }

        private static BfmEventDeviceListener CreateListener(IContainerBuilder components)
        {
            var listener = new BfmEventDeviceListener(
                components.Get<IDevice>(),
                components.Get<IDeserializer<BfmEventDS>>(),
                components.Get<Port<BfmEventDS>>()
            );
            return listener;
        }

        private static JournalEntry.Adapters.Controller CreateController(
            IContainerBuilder components,
            Port<IEnumerable<GlEntry>> glEntryPort)
        {
            var controller = new Controller(
                components.Get<Port<BfmEventDS>>(),
                glEntryPort,
                components.Get<IAccumulator>(),
                components.Get<IMapper<JournalEntryDS, GlEntry>>(),
                components.Get<IInfoServicesGateway>()
            );
            return controller;
        }
    }
}