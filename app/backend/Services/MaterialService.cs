using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepository;

        public MaterialService(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        public async Task<PaginatedResponse<Material>> GetMaterialsPaginatedAsync(int companyId, PaginationQuery query)
        {
            var (items, totalCount) = await _materialRepository.GetMaterialsPaginatedAsync(
                companyId, query.Offset, query.PageSize, query.Search);

            return new PaginatedResponse<Material>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<MaterialDetailDto?> GetMaterialDetailAsync(int companyId, int id)
        {
            var material = await _materialRepository.GetMaterialByIdAsync(companyId, id);
            if (material == null) return null;

            var transactions = await _materialRepository.GetTransactionsByMaterialAsync(companyId, id);

            return new MaterialDetailDto
            {
                Material = material,
                RecentTransactions = transactions
            };
        }

        public async Task<Material> CreateMaterialAsync(int companyId, CreateMaterialDto dto)
        {
            var material = new Material
            {
                CompanyId = companyId,
                Name = dto.Name,
                Unit = dto.Unit,
                CurrentStock = 0,
                MinStock = dto.MinStock,
                LastPrice = 0
            };

            var id = await _materialRepository.CreateMaterialAsync(material);
            material.Id = id;
            return material;
        }

        public async Task<Material?> UpdateMaterialAsync(int companyId, int id, UpdateMaterialDto dto)
        {
            var existing = await _materialRepository.GetMaterialByIdAsync(companyId, id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Unit = dto.Unit;
            existing.MinStock = dto.MinStock;

            var success = await _materialRepository.UpdateMaterialAsync(existing);
            return success ? existing : null;
        }

        public async Task<bool> UpdateImageUrlAsync(int companyId, int id, string imageUrl)
        {
            return await _materialRepository.UpdateImageUrlAsync(companyId, id, imageUrl);
        }

        public async Task<bool> DeleteMaterialAsync(int companyId, int id)
        {
            return await _materialRepository.DeleteMaterialAsync(companyId, id);
        }

        public async Task<IEnumerable<Material>> GetLowStockMaterialsAsync(int companyId)
        {
            return await _materialRepository.GetLowStockMaterialsAsync(companyId);
        }

        public async Task<MaterialTransaction> RecordTransactionAsync(int companyId, CreateMaterialTransactionDto dto)
        {
            var material = await _materialRepository.GetMaterialByIdAsync(companyId, dto.MaterialId);
            if (material == null)
                throw new Exception("Material not found or unauthorized.");

            var validTypes = new[] { "purchase_in", "requisition_out", "return", "adjustment" };
            if (!validTypes.Contains(dto.Type))
                throw new Exception($"Invalid transaction type: {dto.Type}. Valid values: {string.Join(", ", validTypes)}");

            // Calculate new stock
            decimal newStock = material.CurrentStock;
            decimal? lastPrice = null;

            switch (dto.Type)
            {
                case "purchase_in":
                    newStock += dto.Qty;
                    lastPrice = dto.UnitPrice;
                    break;
                case "requisition_out":
                    if (material.CurrentStock < dto.Qty)
                        throw new Exception($"Insufficient stock. Current: {material.CurrentStock} {material.Unit}, Requested: {dto.Qty} {material.Unit}");
                    newStock -= dto.Qty;
                    break;
                case "return":
                    newStock += dto.Qty;
                    break;
                case "adjustment":
                    newStock = dto.Qty; // Adjustment sets absolute value
                    break;
            }

            // Create transaction
            var transaction = new MaterialTransaction
            {
                MaterialId = dto.MaterialId,
                ProjectId = dto.ProjectId,
                CompanyId = companyId,
                Type = dto.Type,
                Qty = dto.Qty,
                UnitPrice = dto.UnitPrice,
                Note = dto.Note,
                Date = dto.Date
            };

            var transactionId = await _materialRepository.CreateTransactionAsync(transaction);
            transaction.Id = transactionId;

            // Update material stock
            await _materialRepository.UpdateMaterialStockAsync(dto.MaterialId, newStock, lastPrice);

            transaction.MaterialName = material.Name;
            return transaction;
        }
    }
}
