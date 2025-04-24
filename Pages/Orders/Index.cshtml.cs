using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesOrderApp.Data;
using SalesOrderApp.Models;

namespace SalesOrderApp.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<SO_ORDER> Orders { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? OrderDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1; // Pagination
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5; // End of Pagination

        public async Task OnGetAsync(bool? success)
        {
            if (success == true)
            {
                ViewData["SuccessMessage"] = "Order saved successfully!";
            }

            var query = _context.SO_ORDER
                .Include(o => o.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                query = query.Where(o => o.ORDER_NO.Contains(SearchKeyword) ||
                         (o.Customer != null && o.Customer.CUSTOMER_NAME.Contains(SearchKeyword)));
            }

            if (OrderDate.HasValue)
            {
                query = query.Where(o => o.ORDER_DATE.Date == OrderDate.Value.Date);
            }

            var totalOrders = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);

            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            Orders = await query
                .OrderBy(o => o.ORDER_NO)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public IActionResult OnGetCreate()
        {
            return RedirectToPage("./Edit");
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            var orders = await _context.SO_ORDER
                .Include(o => o.Customer)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;

                // Header
                worksheet.Cell(currentRow, 1).Value = "No";
                worksheet.Cell(currentRow, 2).Value = "Sales Order";
                worksheet.Cell(currentRow, 3).Value = "Order Date";
                worksheet.Cell(currentRow, 4).Value = "Customer";

                // Data
                foreach (var order in orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = currentRow - 1;
                    worksheet.Cell(currentRow, 2).Value = order.ORDER_NO;
                    worksheet.Cell(currentRow, 3).Value = order.ORDER_DATE.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 4).Value = order.Customer?.CUSTOMER_NAME;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
                }
            }
        }
    }
}