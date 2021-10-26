using ClearsBot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //GenericHost
            //using var host  = new HostBuilder()
            //    .ConfigureServices((context, services) => services 
            //    .AddSingleton<ClearsBot.Program>()
            //    .BuildServiceProvider()


            //    )
            //CreateHostBuilder(args).Build().Run();
            _ = Start();
        }

        private static async Task Start()
        {
            using var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddClearsBot();
                }).Build();

            host.Run();
            //await Host.CreateDefaultBuilder()
            //    .ConfigureServices((hostContext, services) =>
            //    {
            //        services
            //        .AddHostedService<ClearsBot.EntryPoint>()
            //        .AddSingleton<ClearsBot.Modules.IRaids, ClearsBot.Modules.Raids>();

            //    }).RunConsoleAsync();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
    }
}
