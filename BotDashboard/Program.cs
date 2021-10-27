using ClearsBot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BotDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var host = CreateHostBuilder().Build();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder()
        {
            return new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder => { 
                    webBuilder
                        .UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddClearsBot();
                })
                .ConfigureLogging((context, builder) => {
                    builder
                        .AddConfiguration(context.Configuration.GetSection("Logging"))
                        .AddConsole();

                    if (context.HostingEnvironment.IsDevelopment())
                        builder.AddDebug();
                })
                .UseContentRoot(Directory.GetCurrentDirectory());
        }
    }
}
