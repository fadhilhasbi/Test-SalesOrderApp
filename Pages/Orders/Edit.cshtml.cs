using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesOrderApp.Data;
using SalesOrderApp.Models;

namespace SalesOrderApp.Pages.Orders
{
    public class Edit : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Edit(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SO_ORDER Order { get; set; } = new SO_ORDER();

        [BindProperty]
        public List<SO_ITEM> Items { get; set; } = new List<SO_ITEM>();

        public List<COM_CUSTOMER> Customers { get; set; } = new List<COM_CUSTOMER>();

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            Customers = await _context.COM_CUSTOMER.ToListAsync();

            if (id == null)
            {
                // Create new order
                Order.ORDER_DATE = DateTime.Today;
                return Page();
            }

            // Edit existing order
            Order = await _context.SO_ORDER
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.SO_ORDER_ID == id)
                ?? new SO_ORDER();

            if (Order == null)
            {
                return NotFound();
            }

            Items = Order.Items;
            return Page();
        }

        public async Task<IActionResult> OnPostAddItemAsync()
        {
            Customers = await _context.COM_CUSTOMER.ToListAsync();
            Items.Add(new SO_ITEM());
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int index)
        {
            Customers = await _context.COM_CUSTOMER.ToListAsync();
            Items.RemoveAt(index);
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Customers = await _context.COM_CUSTOMER.ToListAsync();
                return Page();
            }

            if (Order.SO_ORDER_ID == 0)
            {
                // New order
                _context.SO_ORDER.Add(Order);
                await _context.SaveChangesAsync();

                foreach (var item in Items)
                {
                    item.SO_ORDER_ID = Order.SO_ORDER_ID;
                    _context.SO_ITEM.Add(item);
                }
            }
            else
            {
                // Update existing order
                var existingOrder = await _context.SO_ORDER
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.SO_ORDER_ID == Order.SO_ORDER_ID);

                if (existingOrder == null)
                {
                    return NotFound();
                }

                existingOrder.ORDER_NO = Order.ORDER_NO;
                existingOrder.ORDER_DATE = Order.ORDER_DATE;
                existingOrder.COM_CUSTOMER_ID = Order.COM_CUSTOMER_ID;
                existingOrder.ADDRESS = Order.ADDRESS;

                // Remove existing items
                _context.SO_ITEM.RemoveRange(existingOrder.Items);

                // Add new items
                foreach (var item in Items)
                {
                    item.SO_ORDER_ID = Order.SO_ORDER_ID;
                    _context.SO_ITEM.Add(item);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}