using AutoMapper;
using EdoxoPro.Domain.Entities;
using EdoxoPro.Application.DTOs.Auth;
using EdoxoPro.Application.DTOs.Users;
using EdoxoPro.Application.DTOs.Roles;
using EdoxoPro.Application.DTOs.Contacts;
using EdoxoPro.Application.DTOs.Products;
using EdoxoPro.Application.DTOs.Sales;
using EdoxoPro.Application.DTOs.Purchases;
using EdoxoPro.Application.DTOs.Inventory;
using EdoxoPro.Application.DTOs.Expenses;
using EdoxoPro.Application.DTOs.Checks;
using EdoxoPro.Application.DTOs.Settings;
using EdoxoPro.Application.DTOs.Dashboard;

namespace EdoxoPro.Application.Mapping;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth
        CreateMap<User, UserInfo>();
        CreateMap<RegisterRequest, User>();

        // Users
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        // Roles
        CreateMap<Role, RoleDto>();
        CreateMap<CreateRoleDto, Role>();
        CreateMap<UpdateRoleDto, Role>();

        // Contacts
        CreateMap<Supplier, SupplierDto>();
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<UpdateSupplierDto, Supplier>();

        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();

        CreateMap<CustomerGroup, CustomerGroupDto>();
        CreateMap<CreateCustomerGroupDto, CustomerGroup>();
        CreateMap<UpdateCustomerGroupDto, CustomerGroup>();

        CreateMap<Domain.Entities.Delegate, DelegateDto>();
        CreateMap<CreateDelegateDto, Domain.Entities.Delegate>();
        CreateMap<UpdateDelegateDto, Domain.Entities.Delegate>();

        // Products
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        CreateMap<ProductVariant, ProductVariantDto>();
        CreateMap<CreateProductVariantDto, ProductVariant>();
        CreateMap<UpdateProductVariantDto, ProductVariant>();

        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<CreateProductCategoryDto, ProductCategory>();
        CreateMap<UpdateProductCategoryDto, ProductCategory>();

        CreateMap<ProductBrand, ProductBrandDto>();
        CreateMap<CreateProductBrandDto, ProductBrand>();
        CreateMap<UpdateProductBrandDto, ProductBrand>();

        CreateMap<ProductUnit, ProductUnitDto>();
        CreateMap<CreateProductUnitDto, ProductUnit>();
        CreateMap<UpdateProductUnitDto, ProductUnit>();

        // Sales
        CreateMap<Sale, SaleDto>();
        CreateMap<SaleItem, SaleItemDto>();
        CreateMap<CreateSaleDto, Sale>();
        CreateMap<CreateSaleItemDto, SaleItem>();
        CreateMap<SaleReturn, SaleReturnDto>();
        CreateMap<CreateSaleReturnDto, SaleReturn>();

        // Purchases
        CreateMap<Purchase, PurchaseDto>();
        CreateMap<PurchaseItem, PurchaseItemDto>();
        CreateMap<CreatePurchaseDto, Purchase>();
        CreateMap<CreatePurchaseItemDto, PurchaseItem>();
        CreateMap<PurchaseReturn, PurchaseReturnDto>();
        CreateMap<CreatePurchaseReturnDto, PurchaseReturn>();

        // Inventory
        CreateMap<StockTransfer, StockTransferDto>();
        CreateMap<CreateStockTransferDto, StockTransfer>();
        CreateMap<DamagedStock, DamagedStockDto>();
        CreateMap<CreateDamagedStockDto, DamagedStock>();
        CreateMap<InventoryAudit, InventoryAuditDto>();
        CreateMap<CreateInventoryAuditDto, InventoryAudit>();

        // Expenses
        CreateMap<Expense, ExpenseDto>();
        CreateMap<CreateExpenseDto, Expense>();
        CreateMap<UpdateExpenseDto, Expense>();
        CreateMap<ExpenseCategory, ExpenseCategoryDto>();
        CreateMap<CreateExpenseCategoryDto, ExpenseCategory>();
        CreateMap<UpdateExpenseCategoryDto, ExpenseCategory>();

        // Checks
        CreateMap<Check, CheckDto>();
        CreateMap<CreateCheckDto, Check>();

        // Settings
        CreateMap<CompanySetting, CompanySettingDto>();
        CreateMap<UpdateCompanySettingDto, CompanySetting>();
        CreateMap<InvoiceSetting, InvoiceSettingDto>();
        CreateMap<UpdateInvoiceSettingDto, InvoiceSetting>();
        CreateMap<BarcodeSetting, BarcodeSettingDto>();
        CreateMap<UpdateBarcodeSettingDto, BarcodeSetting>();
        CreateMap<Branch, BranchDto>();
        CreateMap<CreateBranchDto, Branch>();
        CreateMap<UpdateBranchDto, Branch>();
    }
}
