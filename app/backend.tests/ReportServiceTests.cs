using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;
using ConstructionSaaS.Api.Services;
using Moq;
using Xunit;

namespace ConstructionSaaS.Tests
{
    public class ReportServiceTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepo;
        private readonly Mock<IExpenseRepository> _mockExpenseRepo;
        private readonly Mock<IInvoiceRepository> _mockInvoiceRepo;
        private readonly ReportService _service;

        public ReportServiceTests()
        {
            _mockProjectRepo = new Mock<IProjectRepository>();
            _mockExpenseRepo = new Mock<IExpenseRepository>();
            _mockInvoiceRepo = new Mock<IInvoiceRepository>();
            _service = new ReportService(_mockProjectRepo.Object, _mockExpenseRepo.Object, _mockInvoiceRepo.Object);
        }

        [Fact]
        public async Task GetProjectReportAsync_ShouldCalculateCorrectTotals()
        {
            // Arrange
            int companyId = 1;
            int projectId = 10;
            var project = new Project { Id = projectId, ProjectName = "Test", Budget = 10000 };
            var expenses = new List<Expense>
            {
                new Expense { Amount = 1000, Category = "material_cost" },
                new Expense { Amount = 500, Category = "labor_cost" }
            };
            var invoices = new List<Invoice>
            {
                new Invoice { TotalAmount = 15000, Status = "sent" }
            };

            _mockProjectRepo.Setup(r => r.GetProjectByIdAsync(companyId, projectId)).ReturnsAsync(project);
            _mockExpenseRepo.Setup(r => r.GetExpensesByProjectIdAsync(companyId, projectId)).ReturnsAsync(expenses);
            _mockInvoiceRepo.Setup(r => r.GetInvoicesPaginatedAsync(companyId, projectId, 0, 1000, null))
                            .ReturnsAsync((invoices, 1));
            _mockInvoiceRepo.Setup(r => r.GetTotalPaidByInvoiceAsync(It.IsAny<int>())).ReturnsAsync(12000);

            // Act
            var report = await _service.GetProjectReportAsync(companyId, projectId);

            // Assert
            Assert.Equal(1500, report.TotalExpenses);
            Assert.Equal(15000, report.TotalInvoiced);
            Assert.Equal(12000, report.TotalPaid);
            Assert.Equal(3000, report.OutstandingReceivable);
            Assert.Equal(10500, report.Profit); // 12000 - 1500
        }
    }
}
