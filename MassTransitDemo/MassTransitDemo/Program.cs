using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransitDemo
{
    public class Message
    { 
        public string Text { get; set; }
    }


    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition));
                
                x.UsingInMemory();
            });
            services.AddMassTransitHostedService();

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            await busControl.Publish(new SubmitOrder
            {
                OrderId = 1
            });
            
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}