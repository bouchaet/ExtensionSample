using System.Collections.Generic;
using Entities;

namespace ModelBuilder.UseCases
{
    public abstract class ModelBuilderFeature : IFeature
    {
        public string Name => "modelbuilder";
        public abstract IEnumerable<ICommand> Commands { get; }
        public abstract void Enable();
        public abstract void Disable();
    }
}
