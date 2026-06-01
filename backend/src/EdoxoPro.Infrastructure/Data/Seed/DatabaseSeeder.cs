using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Infrastructure.Identity;

namespace EdoxoPro.Infrastructure.Data.Seed;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly RoleManager<AppIdentityRole> _roleManager;
    private readonly UserManager<AppIdentityUser> _userManager;

    public DatabaseSeeder(AppDbContext context, RoleManager<AppIdentityRole> roleManager, UserManager<AppIdentityUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        await SeedDefaultRolesAsync();
        await SeedAdminUserAsync();
        await SeedDefaultBranchAsync();
        await SeedSettingsAsync();
        await SeedSampleDataAsync();
    }

    private async Task SeedDefaultRolesAsync()
    {
        var roles = new[]
        {
            ("Admin", "Full system access", true),
            ("Manager", "Management access", false),
            ("Accountant", "Financial operations", false),
            ("SalesRep", "Sales operations", false),
            ("Purchaser", "Purchase operations", false),
            ("WarehouseKeeper", "Warehouse operations", false),
            ("Cashier", "Cash register operations", false),
            ("Viewer", "Read-only access", false),
        };

        foreach (var (name, description, isSystem) in roles)
        {
            if (!await _roleManager.RoleExistsAsync(name))
            {
                var role = new AppIdentityRole(name)
                {
                    Description = description,
                    IsSystem = isSystem
                };
                await _roleManager.CreateAsync(role);
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = "admin@edoxopro.com";
        var existingUser = await _userManager.FindByEmailAsync(adminEmail);
        if (existingUser != null) return;

        var adminUser = new AppIdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Admin",
            IsActive = true,
            EmailConfirmed = true,
            PasswordHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("Admin@123")))
        };

        _context.Set<AppIdentityUser>().Add(adminUser);
        await _context.SaveChangesAsync();

        var adminRole = await _roleManager.FindByNameAsync("Admin");
        if (adminRole != null)
        {
            _context.Set<IdentityUserRole<int>>().Add(new IdentityUserRole<int> { UserId = adminUser.Id, RoleId = adminRole.Id });
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedDefaultBranchAsync()
    {
        if (_context.Branches.Any()) return;

        _context.Branches.Add(new Branch
        {
            Name = "الفرع الرئيسي",
            IsActive = true
        });
        await _context.SaveChangesAsync();
    }

    private async Task SeedSettingsAsync()
    {
        if (!_context.InvoiceSettings.Any())
        {
            _context.InvoiceSettings.Add(new InvoiceSetting
            {
                Prefix = "INV-",
                NextNumber = 1,
                TaxRate = 15,
                ShowTax = true,
                ShowDiscount = true
            });
        }

        if (!_context.CompanySettings.Any())
        {
            _context.CompanySettings.Add(new CompanySetting
            {
                CompanyName = "EdoxoPro ERP",
                TaxNumber = "000-000-000"
            });
        }

        if (!_context.BarcodeSettings.Any())
        {
            _context.BarcodeSettings.Add(new BarcodeSetting
            {
                Format = "CODE128",
                Width = 300,
                Height = 100,
                IsActive = true
            });
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedSampleDataAsync()
    {
        if (_context.ProductCategories.Any()) return;

        var categories = new List<ProductCategory>
        {
            new() { Name = "Electronics", IsActive = true },
            new() { Name = "Clothing", IsActive = true },
            new() { Name = "Food & Beverages", IsActive = true },
            new() { Name = "Office Supplies", IsActive = true },
            new() { Name = "Hardware", IsActive = true },
        };
        _context.ProductCategories.AddRange(categories);

        var brands = new List<ProductBrand>
        {
            new() { Name = "Samsung", IsActive = true },
            new() { Name = "Nike", IsActive = true },
            new() { Name = "Local Brand", IsActive = true },
        };
        _context.ProductBrands.AddRange(brands);

        var units = new List<ProductUnit>
        {
            new() { Name = "Piece", ShortName = "pc", IsActive = true },
            new() { Name = "Kilogram", ShortName = "kg", IsActive = true },
            new() { Name = "Liter", ShortName = "L", IsActive = true },
        };
        _context.ProductUnits.AddRange(units);

        await _context.SaveChangesAsync();

        var products = new List<Product>
        {
            new() { Name = "Smartphone X1", SKU = "PHN-001", Barcode = "10000001", CategoryId = categories[0].Id, BrandId = brands[0].Id, UnitId = units[0].Id, CostPrice = 800, SalePrice = 1200, WholesalePrice = 1000, CurrentStock = 50, MinStock = 10, IsActive = true },
            new() { Name = "Laptop Pro 15", SKU = "LPT-001", Barcode = "10000002", CategoryId = categories[0].Id, BrandId = brands[0].Id, UnitId = units[0].Id, CostPrice = 2500, SalePrice = 3500, WholesalePrice = 3000, CurrentStock = 20, MinStock = 5, IsActive = true },
            new() { Name = "Wireless Headphones", SKU = "AUD-001", Barcode = "10000003", CategoryId = categories[0].Id, BrandId = brands[2].Id, UnitId = units[0].Id, CostPrice = 50, SalePrice = 120, WholesalePrice = 90, CurrentStock = 200, MinStock = 20, IsActive = true },
            new() { Name = "Cotton T-Shirt", SKU = "CLT-001", Barcode = "20000001", CategoryId = categories[1].Id, BrandId = brands[1].Id, UnitId = units[0].Id, CostPrice = 15, SalePrice = 45, WholesalePrice = 30, CurrentStock = 500, MinStock = 50, IsActive = true },
            new() { Name = "Running Shoes", SKU = "SHO-001", Barcode = "20000002", CategoryId = categories[1].Id, BrandId = brands[1].Id, UnitId = units[0].Id, CostPrice = 60, SalePrice = 150, WholesalePrice = 110, CurrentStock = 100, MinStock = 15, IsActive = true },
            new() { Name = "Mineral Water 1L", SKU = "FV-001", Barcode = "30000001", CategoryId = categories[2].Id, BrandId = brands[2].Id, UnitId = units[2].Id, CostPrice = 0.5m, SalePrice = 1.5m, WholesalePrice = 1, CurrentStock = 10000, MinStock = 500, IsActive = true },
            new() { Name = "Printer Paper A4", SKU = "OFF-001", Barcode = "40000001", CategoryId = categories[3].Id, BrandId = brands[2].Id, UnitId = units[1].Id, CostPrice = 3, SalePrice = 8, WholesalePrice = 6, CurrentStock = 1000, MinStock = 100, IsActive = true },
            new() { Name = "Ballpoint Pens (Box)", SKU = "OFF-002", Barcode = "40000002", CategoryId = categories[3].Id, BrandId = brands[2].Id, UnitId = units[0].Id, CostPrice = 2, SalePrice = 6, WholesalePrice = 4.5m, CurrentStock = 500, MinStock = 50, IsActive = true },
            new() { Name = "Hammer 500g", SKU = "HRD-001", Barcode = "50000001", CategoryId = categories[4].Id, BrandId = brands[2].Id, UnitId = units[0].Id, CostPrice = 8, SalePrice = 20, WholesalePrice = 15, CurrentStock = 80, MinStock = 10, IsActive = true },
            new() { Name = "LED Desk Lamp", SKU = "HRD-002", Barcode = "50000002", CategoryId = categories[4].Id, BrandId = brands[0].Id, UnitId = units[0].Id, CostPrice = 18, SalePrice = 40, WholesalePrice = 30, CurrentStock = 150, MinStock = 20, IsActive = true },
        };
        _context.Products.AddRange(products);

        var customers = new List<Customer>
        {
            new() { Name = "Ahmed Ali", Email = "ahmed@example.com", Phone = "0555000001", City = "Riyadh", IsActive = true },
            new() { Name = "Sara Khaled", Email = "sara@example.com", Phone = "0555000002", City = "Jeddah", IsActive = true },
            new() { Name = "Omar Hassan", Email = "omar@example.com", Phone = "0555000003", City = "Dammam", IsActive = true },
            new() { Name = "Layla Nasser", Email = "layla@example.com", Phone = "0555000004", City = "Mecca", IsActive = true },
            new() { Name = "Faisal Abdullah", Email = "faisal@example.com", Phone = "0555000005", City = "Medina", IsActive = true },
        };
        _context.Customers.AddRange(customers);

        var suppliers = new List<Supplier>
        {
            new() { ContactId = "SUP-001", Name = "TechWorld LLC", Email = "info@techworld.com", Phone = "0110000001", City = "Riyadh", IsActive = true },
            new() { ContactId = "SUP-002", Name = "Fashion Hub Co.", Email = "info@fashionhub.com", Phone = "0110000002", City = "Jeddah", IsActive = true },
            new() { ContactId = "SUP-003", Name = "Food Distributors Ltd.", Email = "info@fooddist.com", Phone = "0110000003", City = "Dammam", IsActive = true },
            new() { ContactId = "SUP-004", Name = "OfficeMart Supplies", Email = "info@officemart.com", Phone = "0110000004", City = "Riyadh", IsActive = true },
            new() { ContactId = "SUP-005", Name = "BuildPro Hardware", Email = "info@buildpro.com", Phone = "0110000005", City = "Mecca", IsActive = true },
        };
        _context.Suppliers.AddRange(suppliers);

        await _context.SaveChangesAsync();
    }
}
