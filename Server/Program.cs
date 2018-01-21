using System;
using Entities;
using NonPersistentQueueManager;
using Server.Details;

namespace Server
{
    internal static class Program
    {
        private static void Main()
        {
            ConsoleLogger.Init();
            ConsoleDebug.Init();

            var components = ComponentFactory.CreateComponents();
            components.Get<UseCases.Server>().Start();
            
            //TEST
            var qserver = new QueueServer(
                components.Get<IListener<IDevice>>(),
                components.Get<IQueueManager>()
            );
            qserver.Start();
            //TEST


            Debug.Write("Server is running...");
            Console.WriteLine("Press any key to quit.");
            Console.Read();
        }
    }
}