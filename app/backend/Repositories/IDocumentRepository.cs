using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<DocumentResponseDto>> GetDocumentsByProjectAsync(int companyId, int projectId);
        Task<IEnumerable<DocumentResponseDto>> GetAllDocumentsAsync(int companyId);
        Task<Document?> GetDocumentByIdAsync(int companyId, int id);
        Task<int> CreateDocumentAsync(Document document);
        Task<bool> DeleteDocumentAsync(int companyId, int id);
    }
}
