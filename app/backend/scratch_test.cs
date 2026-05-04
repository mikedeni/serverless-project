using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConstructionSaaS.Api.Data;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Scratch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddJsonFile("/home/peter/Desktop/MyBrick/backend/appsettings.json")
                .Build();
            
            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<DapperContext>();
            services.AddSingleton<IProjectRepository, ProjectRepository>();
            
            var serviceProvider = services.BuildServiceProvider();
            var repo = serviceProvider.GetRequiredService<IProjectRepository>();
            
            Console.WriteLine("Testing ProjectRepository.GetProjectsPaginatedAsync for Company 1...");
            var result = await repo.GetProjectsPaginatedAsync(1, 0, 10, null, null);
            
            foreach (var p in result.Items)
            {
                Console.WriteLine($"Project: {p.ProjectName}, ID: {p.Id}, Budget: {p.Budget}, TotalSpent: {p.TotalSpent}");
            }

            Console.WriteLine("\nTesting ProjectRepository.GetProjectsPaginatedAsync for Company 5...");
            var result5 = await repo.GetProjectsPaginatedAsync(5, 0, 10, null, null);
            
            foreach (var p in result5.Items)
            {
                Console.WriteLine($"Project: {p.ProjectName}, ID: {p.Id}, Budget: {p.Budget}, TotalSpent: {p.TotalSpent}");
            }
        }
    }
}
