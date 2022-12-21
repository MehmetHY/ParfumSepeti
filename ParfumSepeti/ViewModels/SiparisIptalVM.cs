namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class SiparisIptalVM
{
    public class Oge
    {
        public string Urun { get; set; }

        public decimal Fiyat { get; set; }

        public int Adet { get; set; }
    }

    public int Id { get; set; }

    public string OlusturmaTarihi { get; set; }

    public List<Oge> Ogeler { get; set; } = new();

    public string Toplam { get; set; }
}
