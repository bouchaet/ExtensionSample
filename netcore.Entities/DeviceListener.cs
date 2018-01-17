using System;

namespace Entities
{
    public abstract class DeviceListener<T> : IListener, IDeviceProvider
        where T : class
    {
        private readonly IDevice _device;

        public readonly Port<T> OutPort;

        protected DeviceListener(IDevice device,
            Port<T> outPort)
        {
            _device = device;
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

                OutPort.Transfer(Parse(input));
            }
            _device.Close();
            OnShutdown?.Invoke(this, EventArgs.Empty);
        }

        protected abstract T Parse(string s);

        public IDeviceProvider GetDeviceProvider()
        {
            return this;
        }

        public IDevice GetDevice()
        {
            return _device;
        }
    }
}