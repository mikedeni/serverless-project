using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;

namespace ConstructionSaaS.Api.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly IProjectRepository _projectRepository;

        public QuotationService(IQuotationRepository quotationRepository, IProjectRepository projectRepository)
        {
            _quotationRepository = quotationRepository;
            _projectRepository = projectRepository;
        }

        public async Task<PaginatedResponse<Quotation>> GetQuotationsPaginatedAsync(int companyId, int projectId, PaginationQuery query)
        {
            var (items, totalCount) = await _quotationRepository.GetQuotationsPaginatedAsync(
                companyId, projectId, query.Offset, query.PageSize, query.Status);

            return new PaginatedResponse<Quotation>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<QuotationDetailDto?> GetQuotationDetailAsync(int companyId, int id)
        {
            var quotation = await _quotationRepository.GetQuotationByIdAsync(companyId, id);
            if (quotation == null) return null;

            var items = await _quotationRepository.GetQuotationItemsAsync(id);
            var itemList = items.ToList();

            var summary = CalculateSummary(itemList, quotation.MarkupPercent, quotation.Discount, quotation.TaxPercent);

            return new QuotationDetailDto
            {
                Id = quotation.Id,
                ProjectId = quotation.ProjectId,
                QuotationNumber = quotation.QuotationNumber,
                ClientName = quotation.ClientName,
                ClientAddress = quotation.ClientAddress,
                ClientPhone = quotation.ClientPhone,
                Status = quotation.Status,
                MarkupPercent = quotation.MarkupPercent,
                Discount = quotation.Discount,
                TaxPercent = quotation.TaxPercent,
                Note = quotation.Note,
                ValidUntil = quotation.ValidUntil,
                CreatedAt = quotation.CreatedAt,
                Items = itemList,
                Summary = summary
            };
        }

        public async Task<Quotation> CreateQuotationAsync(int companyId, CreateQuotationDto dto)
        {
            // Validate project ownership
            var project = await _projectRepository.GetProjectByIdAsync(companyId, dto.ProjectId);
            if (project == null)
                throw new Exception("Project not found or you do not have permission.");

            // Auto-generate quotation number
            var count = await _quotationRepository.GetQuotationCountByCompanyAsync(companyId);
            var quotationNumber = $"QT-{DateTime.UtcNow:yyyyMM}-{(count + 1):D4}";

            var quotation = new Quotation
            {
                ProjectId = dto.ProjectId,
                CompanyId = companyId,
                QuotationNumber = quotationNumber,
                ClientName = dto.ClientName,
                ClientAddress = dto.ClientAddress,
                ClientPhone = dto.ClientPhone,
                Status = "draft",
                MarkupPercent = dto.MarkupPercent,
                Discount = dto.Discount,
                TaxPercent = dto.TaxPercent,
                Note = dto.Note,
                ValidUntil = dto.ValidUntil
            };

            var quotationId = await _quotationRepository.CreateQuotationAsync(quotation);
            quotation.Id = quotationId;

            // Add items
            for (int i = 0; i < dto.Items.Count; i++)
            {
                var item = dto.Items[i];
                var quotationItem = new QuotationItem
                {
                    QuotationId = quotationId,
                    ItemOrder = i + 1,
                    Description = item.Description,
                    Qty = item.Qty,
                    Unit = item.Unit,
                    UnitPrice = item.UnitPrice
                };
                await _quotationRepository.AddQuotationItemAsync(quotationItem);
            }

            return quotation;
        }

        public async Task<Quotation?> UpdateQuotationAsync(int companyId, int id, UpdateQuotationDto dto)
        {
            var existing = await _quotationRepository.GetQuotationByIdAsync(companyId, id);
            if (existing == null) return null;

            existing.ClientName = dto.ClientName;
            existing.ClientAddress = dto.ClientAddress;
            existing.ClientPhone = dto.ClientPhone;
            existing.MarkupPercent = dto.MarkupPercent;
            existing.Discount = dto.Discount;
            existing.TaxPercent = dto.TaxPercent;
            existing.Note = dto.Note;
            existing.ValidUntil = dto.ValidUntil;

            var success = await _quotationRepository.UpdateQuotationAsync(existing);
            return success ? existing : null;
        }

        public async Task<bool> UpdateQuotationStatusAsync(int companyId, int id, string status)
        {
            var validStatuses = new[] { "draft", "sent", "approved", "rejected" };
            if (!validStatuses.Contains(status))
                throw new Exception($"Invalid status: {status}. Valid values: {string.Join(", ", validStatuses)}");

            return await _quotationRepository.UpdateQuotationStatusAsync(companyId, id, status);
        }

        public async Task<bool> DeleteQuotationAsync(int companyId, int id)
        {
            return await _quotationRepository.DeleteQuotationAsync(companyId, id);
        }

        public async Task<QuotationItem> AddQuotationItemAsync(int companyId, int quotationId, QuotationItemDto dto)
        {
            var quotation = await _quotationRepository.GetQuotationByIdAsync(companyId, quotationId);
            if (quotation == null)
                throw new Exception("Quotation not found or unauthorized.");

            var existingItems = await _quotationRepository.GetQuotationItemsAsync(quotationId);
            var nextOrder = existingItems.Any() ? existingItems.Max(i => i.ItemOrder) + 1 : 1;

            var item = new QuotationItem
            {
                QuotationId = quotationId,
                ItemOrder = nextOrder,
                Description = dto.Description,
                Qty = dto.Qty,
                Unit = dto.Unit,
                UnitPrice = dto.UnitPrice
            };

            var itemId = await _quotationRepository.AddQuotationItemAsync(item);
            item.Id = itemId;
            item.Amount = dto.Qty * dto.UnitPrice;

            return item;
        }

        public async Task<bool> DeleteQuotationItemAsync(int companyId, int quotationId, int itemId)
        {
            var quotation = await _quotationRepository.GetQuotationByIdAsync(companyId, quotationId);
            if (quotation == null)
                throw new Exception("Quotation not found or unauthorized.");

            return await _quotationRepository.DeleteQuotationItemAsync(quotationId, itemId);
        }

        // --- Private helpers ---

        private QuotationSummaryDto CalculateSummary(List<QuotationItem> items, decimal markupPercent, decimal discount, decimal taxPercent)
        {
            var subTotal = items.Sum(i => i.Qty * i.UnitPrice);
            var markupAmount = subTotal * (markupPercent / 100m);
            var afterMarkup = subTotal + markupAmount;
            var afterDiscount = afterMarkup - discount;
            var taxAmount = afterDiscount * (taxPercent / 100m);
            var grandTotal = afterDiscount + taxAmount;

            return new QuotationSummaryDto
            {
                SubTotal = Math.Round(subTotal, 2),
                MarkupAmount = Math.Round(markupAmount, 2),
                DiscountAmount = Math.Round(discount, 2),
                TaxableAmount = Math.Round(afterDiscount, 2),
                TaxAmount = Math.Round(taxAmount, 2),
                GrandTotal = Math.Round(grandTotal, 2)
            };
        }
    }
}
