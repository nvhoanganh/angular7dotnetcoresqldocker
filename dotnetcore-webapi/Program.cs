using System;
using dotnetcore.webapi.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dotnetcore.webapi
{
	public class Program
    {
        public static void Main(string[] args)
        {
			var host = CreateWebHostBuilder(args).Build();

			using (var serviceScope = host.Services.CreateScope())
			{
				serviceScope.ServiceProvider.GetService<DockerDbContext>().Database.Migrate();
				DbSeeder.Initialize(serviceScope.ServiceProvider);
			}
			host.Run();
		}

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
