using System.Collections.Generic;
using System.Threading.Tasks;
using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Repositories;
using ConstructionSaaS.Api.Services;
using Moq;
using Xunit;

namespace ConstructionSaaS.Tests
{
    public class DashboardServiceTests
    {
        private readonly Mock<IDashboardRepository> _mockRepo;
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            _mockRepo = new Mock<IDashboardRepository>();
            _service = new DashboardService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetDashboardMetricsAsync_ShouldCallRepositoryWithCorrectId()
        {
            // Arrange
            int companyId = 1;
            var expectedResponse = new DashboardSummaryResponse { TotalProjects = 5 };
            _mockRepo.Setup(r => r.GetDashboardSummaryAsync(companyId))
                     .ReturnsAsync(expectedResponse);

            // Act
            var result = await _service.GetDashboardMetricsAsync(companyId);

            // Assert
            Assert.Equal(expectedResponse, result);
            _mockRepo.Verify(r => r.GetDashboardSummaryAsync(companyId), Times.Once);
        }

        [Fact]
        public void DashboardSummaryResponse_Calculations_ShouldBeCorrect()
        {
            // Arrange
            var response = new DashboardSummaryResponse
            {
                TotalBudget = 1000,
                TotalExpenses = 250
            };

            // Act & Assert
            Assert.Equal(750, response.TotalRemainingBudget);
            Assert.Equal(25, response.BudgetVsActualPercentage);
        }

        [Fact]
        public void ProjectProgressDto_Calculations_ShouldBeCorrect()
        {
            // Arrange
            var project = new ProjectProgressDto
            {
                Budget = 5000,
                TotalSpent = 1000
            };

            // Act & Assert
            Assert.Equal(20, project.ProgressPercentage);
        }

        [Fact]
        public void BudgetVsActualPercentage_ShouldReturnZero_WhenBudgetIsZero()
        {
            // Arrange
            var response = new DashboardSummaryResponse
            {
                TotalBudget = 0,
                TotalExpenses = 500
            };

            // Act & Assert
            Assert.Equal(0, response.BudgetVsActualPercentage);
        }
    }
}
