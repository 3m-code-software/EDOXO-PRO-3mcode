using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Domain.Enums;
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
        await SeedExpenseCategoriesAsync();
        await SeedTransactionDataAsync();
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
                var identityRole = new AppIdentityRole(name)
                {
                    Description = description,
                    IsSystem = isSystem
                };
                await _roleManager.CreateAsync(identityRole);
            }

            if (!_context.RolesConfig.Any(r => r.Name == name))
            {
                _context.RolesConfig.Add(new Role { Name = name, Description = description, IsSystem = isSystem });
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task SeedAdminUserAsync()
    {
        var adminEmail = "admin@edoxopro.com";
        if (_context.Users.Any(u => u.Email == adminEmail)) return;

        var passwordHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("Admin@123")));

        var adminUser = new User
        {
            Username = adminEmail,
            Email = adminEmail,
            FullName = "System Admin",
            PasswordHash = passwordHash,
            IsActive = true
        };
        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        var adminDomainRole = _context.RolesConfig.FirstOrDefault(r => r.Name == "Admin");
        if (adminDomainRole != null)
        {
            _context.UserRolesMap.Add(new UserRole { UserId = adminUser.Id, RoleId = adminDomainRole.Id });
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
        if (_context.Products.Count() >= 5) return;

        var hasCategories = _context.ProductCategories.Any();
        List<ProductCategory>? categories = null;
        List<ProductBrand>? brands = null;
        List<ProductUnit>? units = null;

        if (!hasCategories)
        {
            categories = new List<ProductCategory>
            {
                new() { Name = "Electronics", IsActive = true },
                new() { Name = "Clothing", IsActive = true },
                new() { Name = "Food & Beverages", IsActive = true },
                new() { Name = "Office Supplies", IsActive = true },
                new() { Name = "Hardware", IsActive = true },
            };
            _context.ProductCategories.AddRange(categories);

            brands = new List<ProductBrand>
            {
                new() { Name = "Samsung", IsActive = true },
                new() { Name = "Nike", IsActive = true },
                new() { Name = "Local Brand", IsActive = true },
            };
            _context.ProductBrands.AddRange(brands);

            units = new List<ProductUnit>
            {
                new() { Name = "Piece", ShortName = "pc", IsActive = true },
                new() { Name = "Kilogram", ShortName = "kg", IsActive = true },
                new() { Name = "Liter", ShortName = "L", IsActive = true },
            };
            _context.ProductUnits.AddRange(units);

            await _context.SaveChangesAsync();
        }

        categories ??= await _context.ProductCategories.ToListAsync();
        brands ??= await _context.ProductBrands.ToListAsync();
        units ??= await _context.ProductUnits.ToListAsync();

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
        await _context.SaveChangesAsync();

        var customers = new List<Customer>
        {
            new() { Name = "Ahmed Ali", Email = "ahmed@example.com", Phone = "0555000001", City = "Riyadh", IsActive = true },
            new() { Name = "Sara Khaled", Email = "sara@example.com", Phone = "0555000002", City = "Jeddah", IsActive = true },
            new() { Name = "Omar Hassan", Email = "omar@example.com", Phone = "0555000003", City = "Dammam", IsActive = true },
            new() { Name = "Layla Nasser", Email = "layla@example.com", Phone = "0555000004", City = "Mecca", IsActive = true },
            new() { Name = "Faisal Abdullah", Email = "faisal@example.com", Phone = "0555000005", City = "Medina", IsActive = true },
        };
        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();

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

    private async Task SeedExpenseCategoriesAsync()
    {
        if (_context.ExpenseCategories.Any()) return;

        _context.ExpenseCategories.AddRange(new List<ExpenseCategory>
        {
            new() { Name = "إيجار", Description = "إيجار المكاتب والمخازن", IsActive = true },
            new() { Name = "مرافق", Description = "كهرباء - مياه - غاز", IsActive = true },
            new() { Name = "رواتب", Description = "رواتب الموظفين", IsActive = true },
            new() { Name = "صيانة", Description = "صيانة الأجهزة والمعدات", IsActive = true },
            new() { Name = "تسويق", Description = "إعلانات وتسويق", IsActive = true },
            new() { Name = "نقل", Description = "نقل وشحن", IsActive = true },
            new() { Name = "قرطاسية", Description = "مستلزمات مكتبية", IsActive = true },
            new() { Name = "اتصالات", Description = "فواتير اتصالات وانترنت", IsActive = true },
            new() { Name = "مستلزمات نظافة", Description = "منتجات التنظيف", IsActive = true },
            new() { Name = "أخرى", Description = "مصاريف متنوعة", IsActive = true },
        });
        await _context.SaveChangesAsync();
    }

    private async Task SeedTransactionDataAsync()
    {
        if (_context.Sales.Any()) return;

        var customers = await _context.Customers.ToListAsync();
        var suppliers = await _context.Suppliers.ToListAsync();
        var products = await _context.Products.ToListAsync();
        var expenseCategories = await _context.ExpenseCategories.ToListAsync();
        if (customers.Count == 0 || suppliers.Count == 0 || products.Count == 0 || expenseCategories.Count == 0) return;

        var adminUser = await _context.Users.FirstAsync(u => u.Email == "admin@edoxopro.com");
        var branch = await _context.Branches.FirstAsync();
        var invoiceSetting = await _context.InvoiceSettings.FirstAsync();

        var random = new Random(42);
        var expenseNames = new[] { "إيجار المكتب", "فواتير الكهرباء", "رواتب الموظفين", "صيانة أجهزة", "حملة تسويقية", "تكاليف شحن", "مستلزمات مكتبية", "فواتير اتصالات", "اشتراك إنترنت", "مستلزمات نظافة", "استشارات قانونية", "تأمين", "تدريب موظفين", "ضيافة عملاء", "صيانة سيارات" };

        // ---- Sales (30 invoices over 30 days) ----
        for (int i = 0; i < 30; i++)
        {
            var date = DateTime.UtcNow.AddDays(-(29 - i)).Date.AddHours(10).AddMinutes(random.Next(0, 480));
            var customer = customers[random.Next(customers.Count)];
            var itemCount = random.Next(1, 5);
            var chosenProducts = new HashSet<int>();
            var saleItems = new List<SaleItem>();
            decimal subtotal = 0;

            for (int j = 0; j < itemCount; j++)
            {
                var productIdx = random.Next(products.Count);
                if (!chosenProducts.Add(productIdx) && chosenProducts.Count < products.Count)
                {
                    productIdx = (productIdx + 1) % products.Count;
                    chosenProducts.Add(productIdx);
                }

                var product = products[productIdx];
                var qty = random.Next(1, 15);
                var unitPrice = product.SalePrice;
                var lineTotal = qty * unitPrice;
                subtotal += lineTotal;

                saleItems.Add(new SaleItem
                {
                    ProductId = product.Id,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    Total = lineTotal
                });
            }

            var tax = Math.Round(subtotal * 0.15m, 2);
            var total = subtotal + tax;

            var status = i < 27 ? SaleStatus.Confirmed : SaleStatus.Draft;
            var paymentStatus = i switch
            {
                < 10 => PaymentStatus.Paid,
                < 20 => PaymentStatus.Unpaid,
                _ => PaymentStatus.Partial
            };
            var shippingStatus = i switch
            {
                < 5 => ShippingStatus.Pending,
                < 15 => ShippingStatus.Shipped,
                _ => ShippingStatus.Delivered
            };

            var paidAmount = paymentStatus switch
            {
                PaymentStatus.Paid => total,
                PaymentStatus.Partial => Math.Round(total * 0.5m, 2),
                _ => 0
            };

            _context.Sales.Add(new Sale
            {
                InvoiceNumber = $"INV-{invoiceSetting.NextNumber + i}",
                CustomerId = customer.Id,
                BranchId = branch.Id,
                Date = date,
                Subtotal = subtotal,
                Discount = 0,
                DiscountType = "Fixed",
                Tax = tax,
                TaxRate = 15,
                Total = total,
                PaidAmount = paidAmount,
                Status = status,
                PaymentStatus = paymentStatus,
                ShippingStatus = shippingStatus,
                Items = saleItems
            });
        }

        // ---- Purchases (15 invoices over 30 days) ----
        for (int i = 0; i < 15; i++)
        {
            var date = DateTime.UtcNow.AddDays(-(29 - i * 2)).Date.AddHours(9).AddMinutes(random.Next(0, 480));
            var supplier = suppliers[random.Next(suppliers.Count)];
            var itemCount = random.Next(2, 6);
            var chosenProducts = new HashSet<int>();
            var purchaseItems = new List<PurchaseItem>();
            decimal subtotal = 0;

            for (int j = 0; j < itemCount; j++)
            {
                var productIdx = random.Next(products.Count);
                if (!chosenProducts.Add(productIdx) && chosenProducts.Count < products.Count)
                {
                    productIdx = (productIdx + 1) % products.Count;
                    chosenProducts.Add(productIdx);
                }
                var product = products[productIdx];
                var qty = random.Next(10, 100);
                var unitPrice = product.CostPrice;
                var lineTotal = qty * unitPrice;
                subtotal += lineTotal;

                purchaseItems.Add(new PurchaseItem
                {
                    ProductId = product.Id,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    Total = lineTotal
                });
            }

            var tax = Math.Round(subtotal * 0.15m, 2);
            var total = subtotal + tax;

            _context.Purchases.Add(new Purchase
            {
                ReferenceNumber = $"PO-{i + 1:D4}",
                SupplierId = supplier.Id,
                BranchId = branch.Id,
                Date = date,
                Subtotal = subtotal,
                Tax = tax,
                TaxRate = 15,
                Total = total,
                PaidAmount = i % 2 == 0 ? total : 0,
                Status = PurchaseStatus.Received,
                PaymentPeriod = random.Next(0, 2) == 0 ? 30 : null,
                Items = purchaseItems
            });
        }

        // ---- Expenses (25 expenses over 30 days) ----
        for (int i = 0; i < 25; i++)
        {
            var date = DateTime.UtcNow.AddDays(-random.Next(0, 30)).Date.AddHours(8).AddMinutes(random.Next(0, 480));
            _context.Expenses.Add(new Expense
            {
                CategoryId = expenseCategories[random.Next(expenseCategories.Count)].Id,
                Amount = Math.Round((decimal)(random.NextDouble() * 9000 + 100), 2),
                Date = date,
                Description = expenseNames[random.Next(expenseNames.Length)],
                BranchId = branch.Id,
                AddedByUserId = adminUser.Id,
                PaymentMethod = random.Next(0, 2) == 0 ? "Cash" : "Bank Transfer"
            });
        }

        await _context.SaveChangesAsync();

        // ---- Update invoice next number ----
        invoiceSetting.NextNumber += 30;
        await _context.SaveChangesAsync();

        // ---- Update stock to reflect sales/purchases ----
        foreach (var product in products)
        {
            var soldQty = await _context.SaleItems
                .Where(si => si.ProductId == product.Id)
                .SumAsync(si => si.Quantity);
            var purchasedQty = await _context.PurchaseItems
                .Where(pi => pi.ProductId == product.Id)
                .SumAsync(pi => pi.Quantity);
            product.CurrentStock = product.CurrentStock + purchasedQty - soldQty;
        }

        // ---- Set some products as low stock for inventory alerts ----
        products[0].CurrentStock = 2;  // Smartphone X1 - low stock
        products[1].CurrentStock = 3;  // Laptop Pro 15 - low stock
        products[8].CurrentStock = 5;  // Hammer - low stock

        await _context.SaveChangesAsync();
    }
}
