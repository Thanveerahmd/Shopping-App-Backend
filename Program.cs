using System;
using System.IO;
using AutoMapper.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Project.Data;
using Project.Entities;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //BuildWebHost(args).Run();
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
                    Seed.SeedRoles(roleManager);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            host.Run();

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:4009")
                .Build();
    }
}
