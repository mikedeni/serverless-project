using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentResponseDto>> GetDocumentsByProjectAsync(int companyId, int projectId);
        Task<IEnumerable<DocumentResponseDto>> GetAllDocumentsAsync(int companyId);
        Task<DocumentResponseDto> UploadDocumentAsync(int companyId, int userId, UploadDocumentDto dto);
        Task<bool> DeleteDocumentAsync(int companyId, int id);
    }
}
