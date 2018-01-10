using System;
using Entities;

namespace BfmEvent.Adapters
{
    public class BfmEventDeviceListener : IListener
    {
        private readonly IDevice _device;

        public readonly Port<Entities.BfmEvent> OutPort;
        private readonly IDeserializer<Entities.BfmEvent> _deserializer;

        public BfmEventDeviceListener(IDevice device,
            IDeserializer<Entities.BfmEvent> deserializer,
            Port<Entities.BfmEvent> outPort)
        {
            _device = device;
            _deserializer = deserializer;
            OutPort = outPort;
        }

        public event EventHandler<EventArgs> OnShutdown;

        public void Listen()
        {
            _device.Open();
            while (true)
            {
                var input = _device.ReadLine();
                if (input == "quit")
                    break;

                var bfmEvent = _deserializer.Deserialize(input);
                OutPort.Transfer(bfmEvent);
            }
            _device.Close();
            OnShutdown?.Invoke(this, EventArgs.Empty);
        }
    }
}