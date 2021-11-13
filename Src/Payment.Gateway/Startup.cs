using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Payment.Communication.RabbitMq;
using Payment.EventSourcing;
using Payment.EventSourcing.Config;

namespace Payment.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var rabbitMqConfig = Configuration.GetSection("rabbitMq");
            var databaseConfig = Configuration.GetSection("database");
            services.Configure<RabbitMqConfig>(rabbitMqConfig);
            services.Configure<Database>(databaseConfig);
            
            
            
            services.AddControllers();
            services.AddLogging();
            services.AddTransient<RabbitMqPublisher>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Payment.Gateway", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment.Gateway v1"));
            // }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}