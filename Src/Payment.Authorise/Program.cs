using AuthorizeService.Factories;
using AuthorizeService.Repository;
using AuthorizeService.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing;
using Payment.EventSourcing.Repository;

namespace AuthorizeService
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
                    services.Configure<RabbitMqConfig>(rabbitMqConfig);
                    services.AddTransient<RabbitMqConsumer>();
                    services.AddTransient<RabbitMqPublisher>();
                    services.AddTransient<AuthorisationFactory>();
                    services.AddTransient<AuthoriseService>();
                    services.AddTransient<EventStore>();
                    services.AddHostedService<Worker>();
                    services.AddMediatR(typeof(Program));
                    services.AddTransient<CanValidateCreditCard, ValidationService>();
                    services.AddTransient<CanValidateMerchant, ValidationService>();
                    services.AddTransient<MerchantRepository>();
                    services.AddTransient<IEventRepository, EventRepository>();
                });
    }
}