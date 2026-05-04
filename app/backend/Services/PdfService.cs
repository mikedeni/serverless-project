using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ConstructionSaaS.Api.DTOs;

namespace ConstructionSaaS.Api.Services
{
    public class PdfService
    {
        public byte[] GenerateProjectReport(ProjectReportDto report)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("MyBrick").Bold().FontSize(24).FontColor(Colors.Indigo.Darken2);
                                c.Item().Text("Construction Project Report").FontSize(14).FontColor(Colors.Grey.Medium);
                            });
                            row.ConstantItem(120).AlignRight().Text(DateTime.Now.ToString("dd MMM yyyy")).FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                        col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    // Content
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Project Info
                        col.Item().Text($"{report.ProjectName}").Bold().FontSize(18);
                        col.Item().Text($"Status: {report.Status.ToUpper()}").FontSize(12).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingVertical(12);

                        // Financial Summary
                        col.Item().Text("Financial Summary").Bold().FontSize(14).FontColor(Colors.Indigo.Darken2);
                        col.Item().PaddingVertical(6);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            AddTableRow(table, "Budget", $"฿{report.Budget:N2}");
                            AddTableRow(table, "Total Expenses", $"฿{report.TotalExpenses:N2}", Colors.Red.Darken1);
                            AddTableRow(table, "Total Invoiced", $"฿{report.TotalInvoiced:N2}");
                            AddTableRow(table, "Total Paid (Received)", $"฿{report.TotalPaid:N2}", Colors.Green.Darken1);
                            AddTableRow(table, "Outstanding Receivable", $"฿{report.OutstandingReceivable:N2}", Colors.Orange.Darken1);
                            AddTableRowBold(table, "Profit / Loss", $"฿{report.Profit:N2}",
                                report.Profit >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2);
                            AddTableRow(table, "Profit Margin", $"{report.ProfitMarginPercent}%");
                        });

                        col.Item().PaddingVertical(16);

                        // Expense Breakdown
                        if (report.ExpenseBreakdown.Any())
                        {
                            col.Item().Text("Expense Breakdown").Bold().FontSize(14).FontColor(Colors.Indigo.Darken2);
                            col.Item().PaddingVertical(6);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Padding(6).Text("Category").Bold().FontSize(10).FontColor(Colors.Grey.Darken1);
                                    header.Cell().Padding(6).AlignRight().Text("Amount").Bold().FontSize(10).FontColor(Colors.Grey.Darken1);
                                    header.Cell().Padding(6).AlignRight().Text("% of Total").Bold().FontSize(10).FontColor(Colors.Grey.Darken1);
                                });

                                foreach (var item in report.ExpenseBreakdown)
                                {
                                    table.Cell().Padding(6).Text(item.Category.Replace("_cost", "").ToUpper()).FontSize(10);
                                    table.Cell().Padding(6).AlignRight().Text($"฿{item.Amount:N2}").FontSize(10);
                                    table.Cell().Padding(6).AlignRight().Text($"{item.Percentage}%").FontSize(10).FontColor(Colors.Grey.Medium);
                                }
                            });
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated by MyBrick SaaS • Page ").FontSize(9).FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateInvoicePdf(InvoiceDetailDto invoice)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("INVOICE").Bold().FontSize(28).FontColor(Colors.Indigo.Darken2);
                                c.Item().Text($"#{invoice.InvoiceNumber}").FontSize(14).FontColor(Colors.Grey.Medium);
                            });
                            row.ConstantItem(140).AlignRight().Column(c =>
                            {
                                c.Item().Text("MyBrick").Bold().FontSize(16).FontColor(Colors.Indigo.Darken2);
                                c.Item().Text("Construction SaaS").FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                        });
                        col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    // Content
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Client & Invoice Info
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Bill To:").Bold().FontSize(10).FontColor(Colors.Grey.Medium);
                                c.Item().Text(invoice.ClientName).Bold().FontSize(14);
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text($"Date: {invoice.CreatedAt:dd MMM yyyy}").FontSize(10);
                                if (invoice.DueDate.HasValue)
                                    c.Item().Text($"Due: {invoice.DueDate.Value:dd MMM yyyy}").FontSize(10).FontColor(Colors.Red.Darken1);
                                c.Item().Text($"Status: {invoice.Status.ToUpper()}").Bold().FontSize(10);
                            });
                        });

                        if (!string.IsNullOrEmpty(invoice.MilestoneLabel))
                        {
                            col.Item().PaddingTop(8).Text($"Milestone: {invoice.MilestoneLabel}").FontSize(10).FontColor(Colors.Grey.Medium);
                        }

                        col.Item().PaddingVertical(16);

                        // Amount Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });

                            AddTableRow(table, "Subtotal Amount", $"฿{invoice.Amount:N2}");
                            AddTableRow(table, $"Tax ({invoice.TaxPercent}%)", $"฿{invoice.TaxAmount:N2}");
                            AddTableRowBold(table, "Total Amount", $"฿{invoice.TotalAmount:N2}", Colors.Indigo.Darken2);
                        });

                        col.Item().PaddingVertical(12);

                        // Payment Summary
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });

                            AddTableRow(table, "Total Paid", $"฿{invoice.PaidAmount:N2}", Colors.Green.Darken1);
                            AddTableRowBold(table, "Balance Due", $"฿{invoice.RemainingBalance:N2}",
                                invoice.RemainingBalance > 0 ? Colors.Red.Darken1 : Colors.Green.Darken1);
                        });

                        if (!string.IsNullOrEmpty(invoice.Description))
                        {
                            col.Item().PaddingTop(16).Text("Description:").Bold().FontSize(10).FontColor(Colors.Grey.Medium);
                            col.Item().PaddingTop(4).Text(invoice.Description).FontSize(10);
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text("Generated by MyBrick SaaS").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateQuotationPdf(QuotationDetailDto quotation)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("QUOTATION").Bold().FontSize(28).FontColor(Colors.Indigo.Darken2);
                                c.Item().Text($"#{quotation.QuotationNumber}").FontSize(14).FontColor(Colors.Grey.Medium);
                            });
                            row.ConstantItem(140).AlignRight().Column(c =>
                            {
                                c.Item().Text("MyBrick").Bold().FontSize(16).FontColor(Colors.Indigo.Darken2);
                                c.Item().Text("Construction SaaS").FontSize(10).FontColor(Colors.Grey.Medium);
                            });
                        });
                        col.Item().PaddingVertical(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    // Content
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Client Info
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Quote To:").Bold().FontSize(10).FontColor(Colors.Grey.Medium);
                                c.Item().Text(quotation.ClientName).Bold().FontSize(14);
                                if (!string.IsNullOrEmpty(quotation.ClientAddress))
                                    c.Item().Text(quotation.ClientAddress).FontSize(10);
                                if (!string.IsNullOrEmpty(quotation.ClientPhone))
                                    c.Item().Text($"Phone: {quotation.ClientPhone}").FontSize(10);
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text($"Date: {quotation.CreatedAt:dd MMM yyyy}").FontSize(10);
                                if (quotation.ValidUntil.HasValue)
                                    c.Item().Text($"Valid Until: {quotation.ValidUntil.Value:dd MMM yyyy}").FontSize(10);
                                c.Item().Text($"Status: {quotation.Status.ToUpper()}").Bold().FontSize(10);
                            });
                        });

                        col.Item().PaddingVertical(16);

                        // Items Table (BOQ)
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(5);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Padding(4).Text("#").Bold();
                                header.Cell().Padding(4).Text("Description").Bold();
                                header.Cell().Padding(4).AlignRight().Text("Qty").Bold();
                                header.Cell().Padding(4).Text("Unit").Bold();
                                header.Cell().Padding(4).AlignRight().Text("Unit Price").Bold();
                                header.Cell().Padding(4).AlignRight().Text("Amount").Bold();
                                
                                header.Cell().ColumnSpan(6).PaddingVertical(4).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                            });

                            int index = 1;
                            foreach (var item in quotation.Items)
                            {
                                table.Cell().Padding(4).Text(index++.ToString());
                                table.Cell().Padding(4).Text(item.Description);
                                table.Cell().Padding(4).AlignRight().Text(item.Qty.ToString("N2"));
                                table.Cell().Padding(4).Text(item.Unit);
                                table.Cell().Padding(4).AlignRight().Text($"฿{item.UnitPrice:N2}");
                                table.Cell().Padding(4).AlignRight().Text($"฿{item.Amount:N2}");
                            }
                        });

                        col.Item().PaddingVertical(12);

                        // Summary
                        col.Item().Row(row => {
                            row.RelativeItem(2).Column(c => {
                                if (!string.IsNullOrEmpty(quotation.Note))
                                {
                                    c.Item().Text("Note:").Bold().FontSize(10).FontColor(Colors.Grey.Medium);
                                    c.Item().Text(quotation.Note).FontSize(10);
                                }
                            });
                            row.RelativeItem(2).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                AddTableRow(table, "Subtotal", $"฿{quotation.Summary.SubTotal:N2}");
                                if (quotation.MarkupPercent > 0)
                                    AddTableRow(table, $"Markup ({quotation.MarkupPercent}%)", $"฿{quotation.Summary.MarkupAmount:N2}", Colors.Green.Darken1);
                                if (quotation.Discount > 0)
                                    AddTableRow(table, "Discount", $"฿{quotation.Summary.DiscountAmount:N2}", Colors.Red.Darken1);
                                
                                AddTableRow(table, $"Tax ({quotation.TaxPercent}%)", $"฿{quotation.Summary.TaxAmount:N2}");
                                AddTableRowBold(table, "Grand Total", $"฿{quotation.Summary.GrandTotal:N2}", Colors.Indigo.Darken2);
                            });
                        });
                    });

                    // Footer
                    page.Footer().AlignCenter().Text("Generated by MyBrick SaaS").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }

        // --- Helper Methods ---

        private static void AddTableRow(TableDescriptor table, string label, string value, string? color = null)
        {
            table.Cell().Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                .Text(label).FontSize(11);
            table.Cell().Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten3)
                .AlignRight().Text(value).FontSize(11).FontColor(color ?? Colors.Grey.Darken3);
        }

        private static void AddTableRowBold(TableDescriptor table, string label, string value, string color)
        {
            table.Cell().Padding(8).Background(Colors.Grey.Lighten4)
                .Text(label).Bold().FontSize(12);
            table.Cell().Padding(8).Background(Colors.Grey.Lighten4)
                .AlignRight().Text(value).Bold().FontSize(12).FontColor(color);
        }
    }
}
