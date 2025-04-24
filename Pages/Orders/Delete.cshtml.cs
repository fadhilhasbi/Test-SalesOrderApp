using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesOrderApp.Data;
using SalesOrderApp.Models;

namespace SalesOrderApp.Pages.Orders
{
    public class Delete : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Delete(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SO_ORDER Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order = await _context.SO_ORDER
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.SO_ORDER_ID == id)
                ?? throw new InvalidOperationException("Order not found");

            if (Order == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.SO_ORDER
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SO_ORDER_ID == id);

            if (order != null)
            {
                _context.SO_ITEM.RemoveRange(order.Items);
                _context.SO_ORDER.Remove(order);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });
        }
    }
}