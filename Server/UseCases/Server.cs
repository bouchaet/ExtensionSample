using Entities;

namespace Server.UseCases
{
    internal abstract class Server
    {
        private readonly IContainerBuilder _components;
        private ISwitchable _switch;

        protected Server(IContainerBuilder components)
        {
            _components = components;
        }

        public void Start()
        {
            if (_switch == null)
                _switch = BuildSwitch(_components);

            _switch.TurnOn();
        }

        protected abstract ISwitchable BuildSwitch(IContainerBuilder components);
    }
}
