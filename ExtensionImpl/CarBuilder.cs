using System.Collections.Specialized;
using Entities;

namespace ExtensionImpl
{
    public class CarBuilder : IModelBuilder
    {
        public string Name => "Car";
        public IModel BuildModel(NameObjectCollectionBase kwArgs)
        {
            return new CarModel();
        }
    }
}