using ConstructionSaaS.Api.Data;
using ConstructionSaaS.Api.Monitoring;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConstructionSaaS.Api.Services
{
    public class MetricsBackgroundService : BackgroundService
    {
        private readonly DapperContext _context;
        private readonly ILogger<MetricsBackgroundService> _logger;

        public MetricsBackgroundService(DapperContext context, ILogger<MetricsBackgroundService> logger)
        {
            _context = context;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateMetricsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Prometheus metrics");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task UpdateMetricsAsync()
        {
            using var connection = _context.CreateConnection();

            var projectCount = await connection.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Projects");
            BusinessMetrics.ProjectCount.Set(projectCount);

            var totalExpenses = await connection.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Amount), 0) FROM Expenses");
            BusinessMetrics.TotalExpenseAmount.Set((double)totalExpenses);
            
            _logger.LogInformation("Prometheus business metrics updated.");
        }
    }
}
