using System.Collections.Generic;
using System.Linq;
using Entities;

namespace Administration.UseCases
{
    internal class ListCommand : ICommand
    {
        private readonly FeatureCommandCollection _coll;
        public Port<IList<string>> OutPort { get; }

        internal ListCommand(FeatureCommandCollection coll, Port<IList<string>> outputPort)
        {
            OutPort = outputPort;
            _coll = coll;
        }

        public string Name => "list";

        public void Execute()
        {
            var commandNames = _coll
                .Select(feature => feature.Value.Aggregate(
                    $"{feature.Key} <",
                    (current, next) => $"{current}{next.Name}|",
                    name => $"{name.Remove(name.Length -1, 1)}>"))
                .ToList();

            OutPort?.Transfer(commandNames);
        }
    }
}