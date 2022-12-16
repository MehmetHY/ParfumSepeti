namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class UrunListeleItem
{
    public int Id { get; set; }

    public string KapakUrl { get; set; }

    public string Baslik { get; set; }

    public string Model { get; set; }

    public decimal Fiyat { get; set; }

    public int Indirim { get; set; }

    public string Kategori { get; set; }
}

public class UrunListeleVM : PagedVM<UrunListeleItem>
{
}
