using System.Collections.Generic;
using Administration.Adapters;
using Entities;

namespace Administration.Details
{
    internal class DeviceCommandView : ICommandView
    {
        private readonly IDeviceProvider _provider;

        public DeviceCommandView(IDeviceProvider device)
        {
            _provider = device;
        }

        public void ShowAll(IEnumerable<string> cmds)
        {
            foreach (var cmd in cmds)
                _provider?.GetDevice()?.WriteLine(cmd);
        }
    }
}