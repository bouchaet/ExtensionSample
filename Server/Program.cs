using System;
using System.Runtime.Loader;
using System.Threading;
using Aws.ApplicationIntegration.SimpleQueueService;
using Entities;
using Entities.Http;
using Server.Adapters.Http;
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

            var routetable = new HttpRouteTable();
            routetable.Add("/test/:testname", new DebugResource());
            var httpserver = components.Get<HttpServer>();
            httpserver.RouteTable = routetable;
            var task = httpserver.Start();
            //TEST


            // Debug.Write("Server is running...");
            // Console.WriteLine("Press enter twice to exit.");
            // Console.ReadLine();
            // httpserver.Stop();
            // Console.ReadLine();
            // Debug.Write("Bye bye!");


            var ended = new ManualResetEventSlim();
            var starting = new ManualResetEventSlim();

            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                System.Console.WriteLine("Unloading fired");
                starting.Set();

                System.Console.WriteLine("Waiting for completion");
                ended.Wait();
            };

            System.Console.WriteLine("Waiting for signals");
            starting.Wait();

            System.Console.WriteLine("Received signal gracefully shutting down");
            Thread.Sleep(5000);
            ended.Set();
        }
    }
}