using Entities;

namespace Server
{
    internal static class Program
    {
        private static void Main()
        {
            var components = ComponentFactory.CreateComponents();
            Logger.Set(components.Get<ILogger>());
            components.Get<UseCases.Server>().Start();
        }
    }
}