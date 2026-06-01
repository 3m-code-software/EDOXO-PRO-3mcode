using EdoxoPro.Domain.Enums;

namespace EdoxoPro.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<StockTransfer> StockTransfersFrom { get; set; } = new List<StockTransfer>();
    public ICollection<StockTransfer> StockTransfersTo { get; set; } = new List<StockTransfer>();
}
