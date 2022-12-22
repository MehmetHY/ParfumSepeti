namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class AdminSiparisDetayVM
{
    public class Oge
    {
        public string Urun { get; set; }
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
    }

    public int SiparisId { get; set; }
    public string Kullanici { get; set; }

    public List<Oge> Ogeler { get; set; } = new();
    public string Toplam { get; set; }

    public string OlusturmaTarihi { get; set; }
    public string OdemeDurumu { get; set; }
    public string OdemeTarihi { get; set; }

    public string KargoDurumu { get; set; }
    public string KargoyaVerilmeTarihi { get; set; }
    public string KargoFirmasi { get; set; }
    public string KargoTakipKodu { get; set; }

    public string Isim { get; set; }
    public string SoyIsim { get; set; }
    public string Telefon { get; set; }
    public string Il { get; set; }
    public string Ilce { get; set; }
    public string Adres { get; set; }
    public string PostaKodu { get; set; }
}
