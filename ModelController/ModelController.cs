using System.Linq;
using System.Text;
using Entities;

namespace ModelBuilder
{
    public class ModelController : IUnit<string, string>
    {
        private readonly IManager<IModelBuilder> _modelBuilderMgr;

        public Port<string> InPort { get; }
        public Port<string> OutPort { get; }

        public ModelController(IManager<IModelBuilder> modelBuilderManager,
            Port<string> inPort,
            Port<string> outPort)
        {
            _modelBuilderMgr = modelBuilderManager;

            InPort = inPort;
            InPort.OnDataReceived += ProcessInput;

            OutPort = outPort;
        }

        private void ProcessInput(object sender, string text)
        {
            CreateResponse(text);
        }

        private void CreateResponse(string input)
        {
            if (_modelBuilderMgr == null)
            {
                OutPort.Transfer(string.Empty);
                return;
            }

            var builders = _modelBuilderMgr.Items
                .Where(b => b.Name == input)
                .ToArray();

            if (!builders.Any()) return;

            var response = new StringBuilder();
            foreach (var builder in builders)
                response.Append(builder.BuildModel(null).Compose());

            OutPort.Transfer(response.ToString());
        }
    }
}