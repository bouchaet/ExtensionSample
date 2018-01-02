using Entities;

namespace ExtensionImpl
{
    internal class CarModel : IModel
    {
        public string Name => "CarModel";
        public string Compose()
        {
            return "This is a car model.";
        }
    }
}
