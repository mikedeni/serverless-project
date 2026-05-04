using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;
using ConstructionSaaS.Api.Services;
using Moq;
using Xunit;

namespace ConstructionSaaS.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockRepo;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _mockRepo = new Mock<IProjectRepository>();
            _service = new ProjectService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetProjectsPaginatedAsync_ShouldReturnRepositoryItems()
        {
            // Arrange
            int companyId = 1;
            var query = new PaginationQuery { Page = 1, PageSize = 10 };
            var projects = new List<Project>
            {
                new Project { Id = 1, ProjectName = "Project A", TotalSpent = 500 },
                new Project { Id = 2, ProjectName = "Project B", TotalSpent = 0 }
            };

            _mockRepo.Setup(r => r.GetProjectsPaginatedAsync(companyId, 0, 10, null, null))
                     .ReturnsAsync((projects, 2));

            // Act
            var result = await _service.GetProjectsPaginatedAsync(companyId, query);

            // Assert
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
            Assert.Equal("Project A", result.Items.First().ProjectName);
            Assert.Equal(500, result.Items.First().TotalSpent);
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldEnforceCompanyId()
        {
            // Arrange
            int companyId = 99;
            var project = new Project { ProjectName = "New Project", CompanyId = 1 };
            _mockRepo.Setup(r => r.CreateProjectAsync(It.IsAny<Project>())).ReturnsAsync(1);

            // Act
            await _service.CreateProjectAsync(companyId, project);

            // Assert
            Assert.Equal(companyId, project.CompanyId);
            _mockRepo.Verify(r => r.CreateProjectAsync(It.Is<Project>(p => p.CompanyId == companyId)), Times.Once);
        }
    }
}
