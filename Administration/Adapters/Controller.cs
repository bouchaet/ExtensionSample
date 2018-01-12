using Entities;

namespace Administration.Adapters
{
    internal class Controller
    {
        public Port<ICommand> InputCommandPort { get; set; }

        public Controller(Port<ICommand> inputPort)
        {
            InputCommandPort = inputPort;
            InputCommandPort.OnDataReceived += (sender, command) => ExecuteCommand(command);
        }

        public void ExecuteCommand(ICommand cmd)
        {
            // add to history
            cmd.Execute();
        }
    }
}
