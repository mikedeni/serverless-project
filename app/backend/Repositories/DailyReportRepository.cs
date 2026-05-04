using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class DailyReportRepository : IDailyReportRepository
    {
        private readonly DapperContext _context;

        public DailyReportRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<DailyReport> Items, int TotalCount)> GetReportsPaginatedAsync(
            int companyId, int projectId, int offset, int pageSize, DateTime? startDate, DateTime? endDate)
        {
            using var connection = _context.CreateConnection();
            var whereClause = "WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId";
            if (startDate.HasValue) whereClause += " AND ReportDate >= @StartDate";
            if (endDate.HasValue) whereClause += " AND ReportDate <= @EndDate";

            var countSql = $"SELECT COUNT(*) FROM DailyReports {whereClause};";
            var dataSql = $"SELECT * FROM DailyReports {whereClause} ORDER BY ReportDate DESC LIMIT @PageSize OFFSET @Offset;";

            var parameters = new { CompanyId = companyId, ProjectId = projectId, StartDate = startDate, EndDate = endDate, PageSize = pageSize, Offset = offset };

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<DailyReport>(dataSql, parameters);
            return (items, totalCount);
        }

        public async Task<DailyReport?> GetReportByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<DailyReport>(
                "SELECT * FROM DailyReports WHERE CompanyId = @CompanyId AND Id = @Id LIMIT 1;",
                new { CompanyId = companyId, Id = id });
        }

        public async Task<DailyReport?> GetReportByDateAsync(int companyId, int projectId, DateTime reportDate)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<DailyReport>(
                "SELECT * FROM DailyReports WHERE CompanyId = @CompanyId AND ProjectId = @ProjectId AND ReportDate = @ReportDate LIMIT 1;",
                new { CompanyId = companyId, ProjectId = projectId, ReportDate = reportDate });
        }

        public async Task<int> CreateReportAsync(DailyReport report)
        {
            using var connection = _context.CreateConnection();
            report.CreatedAt = DateTime.UtcNow;
            var sql = @"INSERT INTO DailyReports (ProjectId, CompanyId, ReportDate, Weather, WorkerCount, Summary, Issues, CreatedByUserId, CreatedAt)
                VALUES (@ProjectId, @CompanyId, @ReportDate, @Weather, @WorkerCount, @Summary, @Issues, @CreatedByUserId, @CreatedAt);
                SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(sql, report);
        }

        public async Task<bool> UpdateReportAsync(DailyReport report)
        {
            using var connection = _context.CreateConnection();
            var sql = @"UPDATE DailyReports SET Weather = @Weather, WorkerCount = @WorkerCount, Summary = @Summary, Issues = @Issues
                WHERE Id = @Id AND CompanyId = @CompanyId;";
            return await connection.ExecuteAsync(sql, report) > 0;
        }

        public async Task<bool> DeleteReportAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM DailyReports WHERE Id = @Id AND CompanyId = @CompanyId;", new { Id = id, CompanyId = companyId }) > 0;
        }

        public async Task<IEnumerable<DailyReportPhoto>> GetPhotosByReportAsync(int reportId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<DailyReportPhoto>("SELECT * FROM DailyReportPhotos WHERE DailyReportId = @ReportId ORDER BY Id;", new { ReportId = reportId });
        }

        public async Task<int> AddPhotoAsync(DailyReportPhoto photo)
        {
            using var connection = _context.CreateConnection();
            photo.CreatedAt = DateTime.UtcNow;
            var sql = @"INSERT INTO DailyReportPhotos (DailyReportId, ImageUrl, Caption, CreatedAt)
                VALUES (@DailyReportId, @ImageUrl, @Caption, @CreatedAt); SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<int>(sql, photo);
        }
    }
}
