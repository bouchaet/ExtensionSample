using System.Collections.Generic;
using Entities;

namespace Administration.UseCases
{
    internal class BasicAdminFeature : AdminFeature
    {
        private readonly FeatureCommandCollection _coll;
        private readonly ICollection<ICommand> _commands;
        private readonly IListener _listener;

        public BasicAdminFeature(FeatureCommandCollection coll, IListener listener)
        {
            _coll = coll;
            _commands = new List<ICommand>();
            _listener = listener;
        }

        internal void AddCommand(ICommand cmd)
        {
            _commands.Add(cmd);
        }

        public override void SetFeatureCommandCollection(
            IDictionary<string, IEnumerable<ICommand>> collection)
        {
            _coll.Add(collection);
        }

        public override IEnumerable<ICommand> Commands => _commands;

        public override void Enable()
        {
            _listener.Listen();
        }

        public override void Disable()
        {
        }
    }
}