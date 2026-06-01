namespace EdoxoPro.Application.DTOs.Contacts;

public class CustomerGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DiscountPercent { get; set; }
}
