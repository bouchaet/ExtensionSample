using Entities;

namespace ModelBuilder
{
    internal class BuildModelCommand : ICommand
    {
        public Port<string> OutPort { get; }
        private readonly IDevice _device;

        public BuildModelCommand(IDevice device, Port<string> outputStringtPort)
        {
            OutPort = outputStringtPort;
            _device = device;
        }

        public string Name => "build";
        public void Execute()
        {
            var s = "which model? ";
            _device?.Write(s.ToCharArray(), 0, s.Length);
            var model = _device?.ReadLine();

            OutPort.Transfer(model);
        }
    }
}