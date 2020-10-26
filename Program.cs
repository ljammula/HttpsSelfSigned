using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WeatherForecast
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    HostConfig.CertPath = context.Configuration["CertPath"];
                    HostConfig.CertPassword = context.Configuration["CertPassword"];
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(opt =>
                    {
                        var host = Dns.GetHostEntry("weather.io");
                        opt.Listen(host.AddressList[0], 5000);
                        // opt.ListenAnyIP(5000);
                        // opt.ListenAnyIP(5001, listOpt =>
                        opt.Listen(host.AddressList[0], 5001, listOpt =>
                        {
                                listOpt.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
                            }
                        );
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class HostConfig
    {
    public static string CertPath { get; set; }
    public static string CertPassword { get; set; }
    }
}
