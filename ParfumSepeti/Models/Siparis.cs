using System.Transactions;

namespace ParfumSepeti.Models;

#pragma warning disable CS8618

public class Siparis
{
    public int Id { get; set; }

    public string? KullaniciId { get; set; }
    public Kullanici? Kullanici { get; set; }

    public List<SiparisOgesi> Ogeler { get; set; }

    public DateTime OlusturmaTarihi { get; set; }

    public DateTime? OdemeTarihi { get; set; }

    public DateTime? KargoyaVerilmeTarihi { get; set; }

    public string OdemeDurumu { get; set; }

    public string KargoDurumu { get; set; }

    public string? Isim { get; set; }

    public string? SoyIsim { get; set; }

    public string? Telefon { get; set; }

    public string? Il { get; set; }

    public string? Ilce { get; set; }

    public string? Adres { get; set; }

    public string? PostaKodu { get; set; }

    public string? OdemeIntentId { get; set; }

    public string? SessionId { get; set; }

    public string? KargoSirketi { get; set; }

    public string? KargoTakipKodu { get; set; }
}
