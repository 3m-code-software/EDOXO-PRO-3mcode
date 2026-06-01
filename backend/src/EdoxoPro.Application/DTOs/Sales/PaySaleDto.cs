namespace EdoxoPro.Application.DTOs.Sales;
public class PaySaleDto
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
}
