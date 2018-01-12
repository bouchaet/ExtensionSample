using System.Collections.Generic;
using Entities;

namespace Administration.UseCases
{
    public abstract class AdminFeature : IFeature
    {
        public abstract void SetFeatureCommandCollection(
            IDictionary<string, IEnumerable<ICommand>> collection);

        public abstract IEnumerable<ICommand> Commands { get; }
        public abstract void Enable();
        public abstract void Disable();

        public string Name => "admin";
    }
}