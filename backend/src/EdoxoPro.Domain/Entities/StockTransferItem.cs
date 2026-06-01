namespace EdoxoPro.Domain.Entities;

public class StockTransferItem : BaseEntity
{
    public int TransferId { get; set; }
    public int ProductId { get; set; }
    public double Quantity { get; set; }

    public StockTransfer Transfer { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
