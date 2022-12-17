namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KategoriVM
{
    public string Isim { get; set; }

    public List<UrunCardVM> Urunler { get; set; } = new();
}
