using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{

    /// <summary>
    /// A container is the operating host of components
    /// </summary>
    public interface IContainer
    {
        IEnumerable<IListener> Listeners { get; }
        void StartListening();
        void StopListening();
        IEnumerable<IFeature> HostedFeatures { get; }
    }
}
