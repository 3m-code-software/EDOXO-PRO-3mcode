namespace EdoxoPro.Application.Common;

public class FilterRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; }
    public string SortDirection { get; set; } = "asc";
}
