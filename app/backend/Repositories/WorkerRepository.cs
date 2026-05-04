using Dapper;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Data;

namespace ConstructionSaaS.Api.Repositories
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly DapperContext _context;

        public WorkerRepository(DapperContext context)
        {
            _context = context;
        }

        // --- Workers ---

        public async Task<IEnumerable<Worker>> GetWorkersByCompanyAsync(int companyId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Workers WHERE CompanyId = @CompanyId ORDER BY Name ASC;";
            return await connection.QueryAsync<Worker>(sql, new { CompanyId = companyId });
        }

        public async Task<(IEnumerable<Worker> Items, int TotalCount)> GetWorkersPaginatedAsync(
            int companyId, int offset, int pageSize, string? search, string? status)
        {
            using var connection = _context.CreateConnection();

            var whereClause = "WHERE CompanyId = @CompanyId";
            if (!string.IsNullOrWhiteSpace(search))
                whereClause += " AND (Name LIKE @Search OR Position LIKE @Search)";
            if (!string.IsNullOrWhiteSpace(status))
                whereClause += " AND Status = @Status";

            var countSql = $"SELECT COUNT(*) FROM Workers {whereClause};";
            var dataSql = $"SELECT * FROM Workers {whereClause} ORDER BY Name ASC LIMIT @PageSize OFFSET @Offset;";

            var parameters = new
            {
                CompanyId = companyId,
                Search = $"%{search}%",
                Status = status,
                PageSize = pageSize,
                Offset = offset
            };

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
            var items = await connection.QueryAsync<Worker>(dataSql, parameters);

            return (items, totalCount);
        }

        public async Task<Worker?> GetWorkerByIdAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Workers WHERE CompanyId = @CompanyId AND Id = @Id LIMIT 1;";
            return await connection.QueryFirstOrDefaultAsync<Worker>(sql, new { CompanyId = companyId, Id = id });
        }

        public async Task<int> CreateWorkerAsync(Worker worker)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Workers (CompanyId, Name, Position, DailyWage, Phone, Status, ImageUrl, CreatedAt)
                VALUES (@CompanyId, @Name, @Position, @DailyWage, @Phone, @Status, @ImageUrl, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            worker.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, worker);
        }

        public async Task<bool> UpdateWorkerAsync(Worker worker)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                UPDATE Workers 
                SET Name = @Name, Position = @Position, DailyWage = @DailyWage, 
                    Phone = @Phone, Status = @Status, ImageUrl = @ImageUrl
                WHERE Id = @Id AND CompanyId = @CompanyId;";

            var affectedRows = await connection.ExecuteAsync(sql, worker);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteWorkerAsync(int companyId, int id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM Workers WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }

        // --- Attendances ---

        public async Task<IEnumerable<Attendance>> GetAttendancesByProjectAndDateAsync(int companyId, int projectId, DateTime date)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT a.*, w.Name AS WorkerName, w.DailyWage
                FROM Attendances a
                JOIN Workers w ON a.WorkerId = w.Id
                WHERE a.CompanyId = @CompanyId AND a.ProjectId = @ProjectId AND a.Date = @Date
                ORDER BY w.Name ASC;";

            return await connection.QueryAsync<Attendance>(sql, new { CompanyId = companyId, ProjectId = projectId, Date = date.Date });
        }

        public async Task<int> CreateAttendanceAsync(Attendance attendance)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                INSERT INTO Attendances (WorkerId, ProjectId, CompanyId, Date, CheckIn, CheckOut, OTHours, Note, CreatedAt)
                VALUES (@WorkerId, @ProjectId, @CompanyId, @Date, @CheckIn, @CheckOut, @OTHours, @Note, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            attendance.CreatedAt = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, attendance);
        }

        public async Task<bool> AttendanceExistsAsync(int workerId, int projectId, DateTime date)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Attendances WHERE WorkerId = @WorkerId AND ProjectId = @ProjectId AND Date = @Date;";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { WorkerId = workerId, ProjectId = projectId, Date = date.Date });
            return count > 0;
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByProjectAsync(int companyId, int projectId, DateTime? dateFrom, DateTime? dateTo)
        {
            using var connection = _context.CreateConnection();

            var whereClause = "WHERE a.CompanyId = @CompanyId AND a.ProjectId = @ProjectId";
            if (dateFrom.HasValue)
                whereClause += " AND a.Date >= @DateFrom";
            if (dateTo.HasValue)
                whereClause += " AND a.Date <= @DateTo";

            var sql = $@"
                SELECT a.*, w.Name AS WorkerName, w.DailyWage
                FROM Attendances a
                JOIN Workers w ON a.WorkerId = w.Id
                {whereClause}
                ORDER BY a.Date DESC, w.Name ASC;";

            return await connection.QueryAsync<Attendance>(sql, new
            {
                CompanyId = companyId,
                ProjectId = projectId,
                DateFrom = dateFrom?.Date,
                DateTo = dateTo?.Date
            });
        }

        public async Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE Workers SET ImageUrl = @ImageUrl WHERE Id = @Id AND CompanyId = @CompanyId;";
            var affectedRows = await connection.ExecuteAsync(sql, new { ImageUrl = imageUrl, Id = id, CompanyId = companyId });
            return affectedRows > 0;
        }
    }
}
