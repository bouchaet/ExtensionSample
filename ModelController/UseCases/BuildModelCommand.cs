using Entities;

namespace ModelBuilder.UseCases
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
            const string question = "which model? ";
            _device?.Write(question.ToCharArray(), 0, question.Length);
            var modelName = _device?.ReadLine();

            OutPort.Transfer(modelName);
        }
    }
}