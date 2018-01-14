using Entities;

namespace Server.UseCases
{
    internal class FeatureSwitch : ISwitchable
    {
        private readonly IFeature _feature;

        public FeatureSwitch(IFeature feature)
        {
            _feature = feature;
        }

        public void TurnOn()
        {
            _feature.Enable();
        }

        public void TurnOff()
        {
            _feature.Disable();
        }
    }
}
