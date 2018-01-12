using System.Collections.Generic;
using Administration.Adapters;
using Entities;

namespace Administration.Details
{
    internal class DeviceCommandView : ICommandView
    {
        private readonly IDevice _device;

        public DeviceCommandView(IDevice device)
        {
            _device = device;
        }

        public void ShowAll(IEnumerable<string> cmds)
        {
            foreach (var cmd in cmds)
                _device.WriteLine(cmd);
        }
    }
}