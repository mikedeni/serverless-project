using ConstructionSaaS.Api.Data;
using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using Dapper;

namespace ConstructionSaaS.Api.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DapperContext _context;

        public DocumentRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetDocumentsByProjectAsync(int companyId, int projectId)
        {
            var sql = @"
                SELECT d.*, u.Name as UploadedByUserName 
                FROM Documents d
                LEFT JOIN Users u ON d.UploadedByUserId = u.Id
                WHERE d.CompanyId = @CompanyId AND d.ProjectId = @ProjectId
                ORDER BY d.CreatedAt DESC;";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<DocumentResponseDto>(sql, new { CompanyId = companyId, ProjectId = projectId });
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetAllDocumentsAsync(int companyId)
        {
            var sql = @"
                SELECT d.*, u.Name as UploadedByUserName 
                FROM Documents d
                LEFT JOIN Users u ON d.UploadedByUserId = u.Id
                WHERE d.CompanyId = @CompanyId
                ORDER BY d.CreatedAt DESC;";
            
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<DocumentResponseDto>(sql, new { CompanyId = companyId });
        }

        public async Task<Document?> GetDocumentByIdAsync(int companyId, int id)
        {
            var sql = "SELECT * FROM Documents WHERE CompanyId = @CompanyId AND Id = @Id;";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Document>(sql, new { CompanyId = companyId, Id = id });
        }

        public async Task<int> CreateDocumentAsync(Document document)
        {
            var sql = @"
                INSERT INTO Documents (ProjectId, CompanyId, FileName, FileUrl, FileSize, Category, UploadedByUserId, CreatedAt)
                VALUES (@ProjectId, @CompanyId, @FileName, @FileUrl, @FileSize, @Category, @UploadedByUserId, @CreatedAt);
                SELECT LAST_INSERT_ID();";

            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, document);
        }

        public async Task<bool> DeleteDocumentAsync(int companyId, int id)
        {
            var sql = "DELETE FROM Documents WHERE Id = @Id AND CompanyId = @CompanyId;";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { Id = id, CompanyId = companyId }) > 0;
        }
    }
}
