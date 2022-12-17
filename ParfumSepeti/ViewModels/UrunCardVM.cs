namespace ParfumSepeti.ViewModels;

public class UrunCardVM
{
    public int Id { get; set; }

    public string KapakUrl { get; set; }

    public string Baslik { get; set; }

    public string Model { get; set; }

    public string Kategori { get; set; } = string.Empty;

    public decimal Fiyat { get; set; }

    public int Indirim { get; set; }

    public string IndirimliFiyat => (Fiyat * (100 - Indirim) / 100).ToString("F");
}
