namespace EdoxoPro.Application.DTOs.Dashboard;

public class InventoryAlertDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public double CurrentStock { get; set; }
    public double MinStock { get; set; }
}
