using System;
using Aws.ApplicationIntegration.SimpleQueueService;
using Entities;
using Server.Details;

namespace Server
{
    internal static class Program
    {
        private static void Main()
        {
            ConsoleLogger.Init();
            ConsoleDebug.Init();
            SqsLogger.Init();

            var components = ComponentFactory.CreateComponents();
            components.Get<UseCases.Server>().Start();

            //TEST
            var qserver = new QueueServer(
                components.Get<IListener<IDevice>>(),
                components.Get<IQueueManager>()
            );
            qserver.Start();

            var httpserver = new HttpServer(10867);
            httpserver.Start();
            //TEST


            Debug.Write("Server is running...");
            Console.WriteLine("Press any key to quit.");
            Console.Read();
        }
    }
}