namespace EdoxoPro.Application.DTOs.Settings;
public class UpdateBarcodeSettingDto
{
    public string Symbology { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public bool ShowPrice { get; set; }
    public bool ShowProductName { get; set; }
    public bool IncludeMargin { get; set; }
}
