namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class AnasayfaVM
{
    public class Kategori
    {
        public int Id { get; set; }

        public string Isim { get; set; }

        public List<UrunCardVM> Urunler { get; set; } = new();
    }

    public List<Kategori> Kategoriler { get; set; } = new();

    public List<UrunCardVM> YeniEklenenler { get; set; } = new();

    public List<UrunCardVM> Indirimler { get; set; } = new();
}
