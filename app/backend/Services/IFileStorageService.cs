using Microsoft.AspNetCore.Http;

namespace ConstructionSaaS.Api.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder = "documents");
        bool DeleteFile(string fileUrl);
    }
}
