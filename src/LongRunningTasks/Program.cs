using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Hosting;

namespace LongRunningTasks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder
                        .UseLocalhostClustering()
                        .Configure<ClusterOptions>(opts =>
                        {
                            opts.ClusterId = "LongRunningTasks-dev";
                            opts.ServiceId = "LongRunningTasks";
                        })
                        .Configure<SiloMessagingOptions>(options =>
                        {
                            // reduced message timeout to ease promise break testing
                            options.ClientDropTimeout = TimeSpan.FromSeconds(2);
                            options.ResponseTimeout = TimeSpan.FromSeconds(3);
                            options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(4);
                        })                        
                        .Configure<ClientMessagingOptions>(options =>
                        {
                            // reduced message timeout to ease promise break testing
                            options.ResponseTimeout = TimeSpan.FromSeconds(5);
                            options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(6);
                        })
                        .Configure<EndpointOptions>(opts => { opts.AdvertisedIPAddress = IPAddress.Loopback; });
                });
    }
}
