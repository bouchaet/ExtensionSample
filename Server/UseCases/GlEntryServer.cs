﻿using System;
using System.Collections.Generic;
using System.IO;
using BfmEvent.Adapters;
using Entities;
using JournalEntry.Adapters;
using JournalEntry.Details;
using JournalEntry.UseCases;
using BfmEventDS = Entities.BfmEvent;
using JournalEntryDS = JournalEntry.UseCases.JournalEntry;

namespace Server.UseCases
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
            var listener = CreateListener(components, controller.EventPort);

            listener.Listen();
        }

        private static Port<IEnumerable<PartnerGlEntry>> CreatePresenterPort(IContainerBuilder components)
        {
            var csvView = new CsvView(
                Path.Combine(Environment.CurrentDirectory, "glentries.csv")
            );

            var jsonView = new JsonView(
                Path.Combine(Environment.CurrentDirectory, "glentries.json")
            );

            var presenter = components.Get<IPresenter<PartnerGlEntry>>();
            presenter.AttachView(csvView);
            presenter.AttachView(jsonView);

            return presenter as Port<IEnumerable<PartnerGlEntry>>; //todo: eurk
        }

        private static IListener CreateListener(IContainerBuilder components,
            Port<BfmEventDS> inEventPort)
        {
            var listener = new BfmEventDeviceListener(
                components.Get<IDevice>(),
                components.Get<IDeserializer<BfmEventDS>>(),
                components.Get<Port<BfmEventDS>>()
            );

            listener.OutPort.Connect(inEventPort);
            return listener;
        }

        private static Controller CreateController(
            IContainerBuilder components,
            Port<IEnumerable<PartnerGlEntry>> glEntryPort)
        {
            var controller = new Controller(
                components.Get<Port<BfmEventDS>>(),
                glEntryPort,
                components.Get<IAccumulator>(),
                components.Get<IMapper<JournalEntryDS, PartnerGlEntry>>(),
                components.Get<IInfoServicesGateway>()
            );
            return controller;
        }
    }
}