namespace EdoxoPro.Domain.Entities;

public class Payment : BaseEntity
{
    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public int AddedByUserId { get; set; }
}
