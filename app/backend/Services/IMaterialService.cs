using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IMaterialService
    {
        Task<PaginatedResponse<Material>> GetMaterialsPaginatedAsync(int companyId, PaginationQuery query);
        Task<MaterialDetailDto?> GetMaterialDetailAsync(int companyId, int id);
        Task<Material> CreateMaterialAsync(int companyId, CreateMaterialDto dto);
        Task<Material?> UpdateMaterialAsync(int companyId, int id, UpdateMaterialDto dto);
        Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl);
        Task<bool> DeleteMaterialAsync(int companyId, int id);
        Task<IEnumerable<Material>> GetLowStockMaterialsAsync(int companyId);
        Task<MaterialTransaction> RecordTransactionAsync(int companyId, CreateMaterialTransactionDto dto);
    }
}
