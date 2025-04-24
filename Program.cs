using SalesOrderApp.Data;
using Microsoft.EntityFrameworkCore;
using SalesOrderApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMvc().AddRazorPagesOptions(options => {
    options.Conventions.AddPageRoute("/Orders/Index", "");
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();

        if (!context.COM_CUSTOMER.Any())
        {
            // Seed customers
            var customers = new List<COM_CUSTOMER>
            {
                new COM_CUSTOMER { CUSTOMER_NAME = "PROFES" },
                new COM_CUSTOMER { CUSTOMER_NAME = "TITAN" },
                new COM_CUSTOMER { CUSTOMER_NAME = "DIPS" }
            };
            context.COM_CUSTOMER.AddRange(customers);
            context.SaveChanges();

            // Seed orders with items
            var orders = new List<SO_ORDER>
            {
                new SO_ORDER
                {
                    ORDER_NO = "50_001",
                    ORDER_DATE = new DateTime(2011, 2, 24),
                    COM_CUSTOMER_ID = customers[0].COM_CUSTOMER_ID,
                    ADDRESS = "Professional Address",
                    Items = new List<SO_ITEM>
                    {
                        new SO_ITEM { ITEM_NAME = "KULKAS", QUANTITY = 2, PRICE = 5000000 },
                        new SO_ITEM { ITEM_NAME = "AC", QUANTITY = 3, PRICE = 3000000 },
                        new SO_ITEM { ITEM_NAME = "DESKTOP", QUANTITY = 6, PRICE = 10000000 },
                        new SO_ITEM { ITEM_NAME = "TV", QUANTITY = 1, PRICE = 7900000 }
                    }
                },
                new SO_ORDER
                {
                    ORDER_NO = "50_002",
                    ORDER_DATE = new DateTime(2011, 2, 24),
                    COM_CUSTOMER_ID = customers[1].COM_CUSTOMER_ID,
                    ADDRESS = "Titan Address",
                    Items = new List<SO_ITEM>
                    {
                        new SO_ITEM { ITEM_NAME = "KUKAS", QUANTITY = 2, PRICE = 500000000 },
                        new SO_ITEM { ITEM_NAME = "LAPTOP", QUANTITY = 5, PRICE = 15000000 }
                    }
                },
                new SO_ORDER
                {
                    ORDER_NO = "50_003",
                    ORDER_DATE = new DateTime(2011, 2, 25),
                    COM_CUSTOMER_ID = customers[0].COM_CUSTOMER_ID,
                    ADDRESS = "Professional Branch Office",
                    Items = new List<SO_ITEM>
                    {
                        new SO_ITEM { ITEM_NAME = "MONITOR", QUANTITY = 10, PRICE = 2500000 },
                        new SO_ITEM { ITEM_NAME = "PRINTER", QUANTITY = 3, PRICE = 3500000 }
                    }
                },
                new SO_ORDER
                {
                    ORDER_NO = "50_004",
                    ORDER_DATE = new DateTime(2011, 2, 26),
                    COM_CUSTOMER_ID = customers[2].COM_CUSTOMER_ID,
                    ADDRESS = "Dips Main Office",
                    Items = new List<SO_ITEM>
                    {
                        new SO_ITEM { ITEM_NAME = "PHONE", QUANTITY = 15, PRICE = 1500000 },
                        new SO_ITEM { ITEM_NAME = "TABLET", QUANTITY = 8, PRICE = 4500000 }
                    }
                }
            };
            context.SO_ORDER.AddRange(orders);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
