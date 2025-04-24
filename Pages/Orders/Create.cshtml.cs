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
    public class Create : PageModel
    {
private readonly ApplicationDbContext _context;

    public Create(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public SO_ORDER Order { get; set; } = new SO_ORDER();

    [BindProperty]
    public List<SO_ITEM> Items { get; set; } = new List<SO_ITEM>();

    public List<COM_CUSTOMER> Customers { get; set; } = new List<COM_CUSTOMER>();

    public async Task OnGetAsync()
    {
        Customers = await _context.COM_CUSTOMER.ToListAsync();
        Order.ORDER_DATE = DateTime.Today;
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

        // Save order
        _context.SO_ORDER.Add(Order);
        await _context.SaveChangesAsync();

        // Save items
        foreach (var item in Items)
        {
            item.SO_ORDER_ID = Order.SO_ORDER_ID;
            _context.SO_ITEM.Add(item);
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
    }
}