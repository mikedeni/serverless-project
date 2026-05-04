using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepo;
        private readonly IFileStorageService _fileStorage;

        public DocumentService(IDocumentRepository documentRepo, IFileStorageService fileStorage)
        {
            _documentRepo = documentRepo;
            _fileStorage = fileStorage;
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetDocumentsByProjectAsync(int companyId, int projectId)
        {
            return await _documentRepo.GetDocumentsByProjectAsync(companyId, projectId);
        }

        public async Task<IEnumerable<DocumentResponseDto>> GetAllDocumentsAsync(int companyId)
        {
            return await _documentRepo.GetAllDocumentsAsync(companyId);
        }

        public async Task<DocumentResponseDto> UploadDocumentAsync(int companyId, int userId, UploadDocumentDto dto)
        {
            var fileUrl = await _fileStorage.SaveFileAsync(dto.File);

            var document = new Document
            {
                CompanyId = companyId,
                ProjectId = dto.ProjectId,
                FileName = dto.File.FileName,
                FileUrl = fileUrl,
                FileSize = (int)dto.File.Length,
                Category = dto.Category,
                UploadedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var id = await _documentRepo.CreateDocumentAsync(document);

            return new DocumentResponseDto
            {
                Id = id,
                ProjectId = document.ProjectId,
                FileName = document.FileName,
                FileUrl = document.FileUrl,
                FileSize = document.FileSize,
                Category = document.Category,
                UploadedByUserId = document.UploadedByUserId,
                CreatedAt = document.CreatedAt
            };
        }

        public async Task<bool> DeleteDocumentAsync(int companyId, int id)
        {
            var document = await _documentRepo.GetDocumentByIdAsync(companyId, id);
            if (document == null) return false;

            _fileStorage.DeleteFile(document.FileUrl);

            return await _documentRepo.DeleteDocumentAsync(companyId, id);
        }
    }
}
