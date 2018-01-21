using System;
using Entities;

namespace BfmEvent.Adapters
{
    public class BfmEventDeviceListener : DeviceListener<Entities.BfmEvent>
    {
        private readonly IDeserializer<Entities.BfmEvent> _deserializer;

        public BfmEventDeviceListener(IDevice device,
            IDeserializer<Entities.BfmEvent> deserializer,
            Port<Entities.BfmEvent> outPort)
            : base(device, outPort)
        {
            _deserializer = deserializer;
        }

        //public new event EventHandler<EventArgs> OnShutdown;

        protected override Entities.BfmEvent Parse(string s)
        {
            return _deserializer.Deserialize(s);
        }
    }
}