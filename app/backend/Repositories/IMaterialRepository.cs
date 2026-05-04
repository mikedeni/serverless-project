using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IMaterialRepository
    {
        // Materials
        Task<(IEnumerable<Material> Items, int TotalCount)> GetMaterialsPaginatedAsync(
            int companyId, int offset, int pageSize, string? search);
        Task<Material?> GetMaterialByIdAsync(int companyId, int id);
        Task<int> CreateMaterialAsync(Material material);
        Task<bool> UpdateMaterialAsync(Material material);
        Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl);
        Task<bool> DeleteMaterialAsync(int companyId, int id);
        Task<IEnumerable<Material>> GetLowStockMaterialsAsync(int companyId);
        Task<bool> UpdateMaterialStockAsync(int materialId, decimal newStock, decimal? lastPrice);

        // Transactions
        Task<int> CreateTransactionAsync(MaterialTransaction transaction);
        Task<IEnumerable<MaterialTransaction>> GetTransactionsByMaterialAsync(int companyId, int materialId, int limit = 50);
        Task<IEnumerable<MaterialTransaction>> GetTransactionsByProjectAsync(int companyId, int projectId);
    }
}
