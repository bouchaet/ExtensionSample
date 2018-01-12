using System;
using System.Collections.Generic;
using System.IO;
using Entities;
using JournalEntry.Adapters;
using JournalEntry.Details;
using JournalEntry.UseCases;
using BfmEventDS = Entities.BfmEvent;
using JournalEntryDS = JournalEntry.UseCases.JournalEntry;

namespace JournalEntry
{
    public static class JournalEntryFeatureFactory
    {
        public static JournalEntryFeature Create(IContainerBuilder components)
        {
            var eventCmd = new BfmEventCommand(components.Get<Port<BfmEventDS>>());
            var presenter = CreatePresenterPort(components);
            var controller = CreateController(components, presenter);
            var listener = CreateListener(components, eventCmd.OutPort, controller.EventPort);

            var feature = new BasicJournalEntryFeature(listener, eventCmd);
            return feature;
        }

        private static Port<IEnumerable<GlEntry>> CreatePresenterPort(IContainerBuilder components)
        {
            var csvView = new CsvView(
                Path.Combine(Environment.CurrentDirectory, "glentries.csv")
            );

            var jsonView = new JsonView(
                Path.Combine(Environment.CurrentDirectory, "glentries.json")
            );

            var presenter = components.Get<IPresenter<GlEntry>>();
            presenter.AttachView(csvView);
            presenter.AttachView(jsonView);

            return presenter as Port<IEnumerable<GlEntry>>; //todo: eurk
        }

        private static IListener CreateListener(IContainerBuilder components,
            Port<BfmEventDS> outEventPort,
            Port<BfmEventDS> inEventPort)
        {
            var listener = new BfmEventListener(outEventPort);
            listener.OutPort.Connect(inEventPort);
            return listener;
        }

        private static Controller CreateController(
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