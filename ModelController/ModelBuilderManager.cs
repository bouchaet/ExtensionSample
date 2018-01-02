using System;
using System.Collections.Generic;
using System.Linq;
using Entities;

namespace ModelBuilder
{
    public class ModelBuilderManager : IManager<IModelBuilder>
    {
        private readonly ICollection<IModelBuilder> _modelBuilders;
        public Func<IEnumerable<IModelBuilder>> AdditionalBuildersFunc;
        public IEnumerable<IModelBuilder> AdditionalBuilders;

        public ModelBuilderManager()
        {
            _modelBuilders = new List<IModelBuilder>();
            AdditionalBuildersFunc = () => new List<IModelBuilder>();
        }

        public void Add(IModelBuilder item)
        {
            _modelBuilders.Add(item);
        }

        public IEnumerable<IModelBuilder> Items =>
            _modelBuilders.Concat(AdditionalBuildersFunc());

        public void Clear()
        {
            _modelBuilders.Clear();
        }
    }
}
