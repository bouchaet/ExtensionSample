using System.Collections.Generic;
using System.Linq;
using Entities;

namespace ModelBuilder.UseCases
{
    internal class SimpleModelBuilderFeature : ModelBuilderFeature
    {
        private readonly IList<ICommand> _commands;

        public SimpleModelBuilderFeature(params ICommand[] commands)
        {
            _commands = commands?.ToList() ?? new List<ICommand>();
        }

        public override IEnumerable<ICommand> Commands => _commands;
        public override void Enable()
        {
        }

        public override void Disable()
        {
        }
    }
}
