using System;
using Entities;

namespace ModelBuilder
{
    public class ModelListener : IListener
    {
        private readonly IDevice _device;
        private readonly IUnit<string, string> _controller;

        public ModelListener(IDevice device, IUnit<string, string> controllerUnit)
        {
            _device = device;
            _controller = controllerUnit;

            _controller.OutPort.OnDataSent +=
                (sender, text) => device.WriteLine(text);
        }

        public event EventHandler<EventArgs> OnShutdown;

        public void Listen()
        {
            while (true)
            {
                _device.WriteLine("Enter your model (or quit): ");

                string input;
                if ((input = _device.ReadLine()) == "quit") break;

                _device.WriteLine("Response: ");
                _controller.InPort.Receive(input);
            }

            _device.Close();
            OnShutdown?.Invoke(this, EventArgs.Empty);
        }
    }
}