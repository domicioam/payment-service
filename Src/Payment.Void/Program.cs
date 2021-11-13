using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payment.Capture.Repository;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing.Config;
using Payment.EventSourcing.Repository;
using Payment.Void.Services;
using EventStore = Payment.EventSourcing.EventStore;

namespace Payment.Void
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var rabbitMqConfig = hostContext.Configuration.GetSection("rabbitMq");
                    var databaseConfig = hostContext.Configuration.GetSection("database");
                    services.Configure<RabbitMqConfig>(rabbitMqConfig);
                    services.Configure<Database>(databaseConfig);
                    services.AddTransient<RabbitMqConsumer>();
                    services.AddTransient<RabbitMqPublisher>();
                    services.AddTransient<VoidService>();
                    services.AddTransient<TransactionRepository>();
                    services.AddTransient<IEventRepository, EventRepository>();
                    services.AddMediatR(typeof(Program));
                    services.AddMediatR(typeof(EventStore));
                    services.AddHostedService<Worker>();
                });
    }
}