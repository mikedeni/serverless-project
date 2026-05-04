using ConstructionSaaS.Api.Data;
using ConstructionSaaS.Api.DTOs;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ConstructionSaaS.Api.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationBackgroundService(ILogger<NotificationBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndTriggerNotificationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing NotificationBackgroundService.");
                }

                // Wait 1 minute before checking again
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckAndTriggerNotificationsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DapperContext>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            using var connection = context.CreateConnection();

            // 1. Get all active companies and their admin users
            var sqlAdmins = "SELECT Id, CompanyId FROM Users WHERE Role = 'admin'";
            var admins = await connection.QueryAsync(sqlAdmins);

            // Group by company
            var companyAdmins = admins.GroupBy(a => (int)a.CompanyId)
                                      .ToDictionary(g => g.Key, g => g.Select(a => (int)a.Id).ToList());

            foreach (var companyId in companyAdmins.Keys)
            {
                var userIds = companyAdmins[companyId];

                // --- 1. Low Stock Warning ---
                var sqlLowStock = "SELECT Id, Name, CurrentStock, MinStock FROM Materials WHERE CompanyId = @CompanyId AND MinStock > 0 AND CurrentStock <= MinStock";
                var lowStocks = await connection.QueryAsync(sqlLowStock, new { CompanyId = companyId });
                
                foreach (var item in lowStocks)
                {
                    var relatedUrl = $"/materials/{item.Id}";
                    var title = "Low Stock Alert";
                    var message = $"Material '{item.Name}' is low on stock ({item.CurrentStock} / {item.MinStock}).";
                    
                    if (!await NotificationExists(connection, companyId, "low_stock", relatedUrl))
                    {
                        await NotifyUsers(notificationService, companyId, userIds, "low_stock", title, message, relatedUrl);
                    }
                }

                // --- 2. Overdue Invoices ---
                var sqlOverdueInvoices = "SELECT Id, InvoiceNumber, TotalAmount, DueDate FROM Invoices WHERE CompanyId = @CompanyId AND Status = 'sent' AND DueDate < CURRENT_DATE";
                var overdueInvoices = await connection.QueryAsync(sqlOverdueInvoices, new { CompanyId = companyId });
                
                foreach (var inv in overdueInvoices)
                {
                    var relatedUrl = $"/invoices/{inv.Id}";
                    var title = "Invoice Overdue";
                    var message = $"Invoice '{inv.InvoiceNumber}' for ฿{inv.TotalAmount:N2} is overdue since {((DateTime)inv.DueDate).ToShortDateString()}.";
                    
                    if (!await NotificationExists(connection, companyId, "invoice_overdue", relatedUrl))
                    {
                        await NotifyUsers(notificationService, companyId, userIds, "invoice_overdue", title, message, relatedUrl);
                    }
                }

                // --- 3. Budget Warning (Expenses > 90% of Budget) ---
                var sqlBudget = @"
                    SELECT 
                        p.Id, 
                        p.ProjectName, 
                        p.Budget, 
                        COALESCE(costs.TotalSpent, 0) AS TotalSpent
                    FROM Projects p
                    LEFT JOIN (
                        SELECT ProjectId, SUM(Amount) AS TotalSpent FROM (
                            SELECT ProjectId, Amount FROM Expenses WHERE CompanyId = @CompanyId
                            UNION ALL
                            SELECT ProjectId, (Qty * UnitPrice) FROM MaterialTransactions WHERE CompanyId = @CompanyId AND Type = 'purchase_in'
                            UNION ALL
                            SELECT sc.ProjectId, sp.Amount FROM SubcontractorPayments sp JOIN SubcontractorContracts sc ON sp.ContractId = sc.Id WHERE sp.CompanyId = @CompanyId
                            UNION ALL
                            SELECT a.ProjectId, w.DailyWage FROM Attendances a JOIN Workers w ON a.WorkerId = w.Id WHERE a.CompanyId = @CompanyId
                        ) combined GROUP BY ProjectId
                    ) costs ON p.Id = costs.ProjectId
                    WHERE p.CompanyId = @CompanyId AND p.Budget > 0
                    HAVING TotalSpent > (p.Budget * 0.9)";
                    
                var budgetWarnings = await connection.QueryAsync(sqlBudget, new { CompanyId = companyId });

                foreach (var proj in budgetWarnings)
                {
                    var relatedUrl = $"/projects/{proj.Id}";
                    var title = "Budget Warning";
                    var percentage = Math.Round(((decimal)proj.TotalSpent / (decimal)proj.Budget) * 100, 1);
                    var message = $"Project '{proj.ProjectName}' has used {percentage}% of its budget (฿{proj.TotalSpent:N2} / ฿{proj.Budget:N2}).";
                    
                    if (!await NotificationExists(connection, companyId, "budget_warning", relatedUrl))
                    {
                        await NotifyUsers(notificationService, companyId, userIds, "budget_warning", title, message, relatedUrl);
                    }
                }
            }
        }

        private async Task<bool> NotificationExists(System.Data.IDbConnection connection, int companyId, string type, string relatedUrl)
        {
            var sql = @"
                SELECT COUNT(1) FROM Notifications 
                WHERE CompanyId = @CompanyId 
                  AND Type = @Type 
                  AND RelatedUrl = @RelatedUrl 
                  AND IsRead = 0";
            return await connection.ExecuteScalarAsync<int>(sql, new { CompanyId = companyId, Type = type, RelatedUrl = relatedUrl }) > 0;
        }

        private async Task NotifyUsers(INotificationService notificationService, int companyId, List<int> userIds, string type, string title, string message, string relatedUrl)
        {
            foreach (var userId in userIds)
            {
                await notificationService.CreateNotificationAsync(companyId, new CreateNotificationDto
                {
                    UserId = userId,
                    Type = type,
                    Title = title,
                    Message = message,
                    RelatedUrl = relatedUrl
                });
            }
        }
    }
}
