using Entities;

namespace Administration.Adapters
{
    internal class StringDeviceListener : DeviceListener<string>
    {
        public StringDeviceListener(IDevice device, Port<string> outPort)
            : base(device, outPort)
        {
        }

        protected override string Parse(string s)
        {
            return s;
        }
    }
}
