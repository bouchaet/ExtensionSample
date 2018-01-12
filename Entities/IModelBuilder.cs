using System.Collections.Specialized;

namespace Entities
{
    public interface IModelBuilder
    {
        string Name { get; }
        IModel BuildModel(NameObjectCollectionBase kwArgs);
    }
}
