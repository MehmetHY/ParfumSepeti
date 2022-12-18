using ParfumSepeti.Models;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class UrunVM
{
    public int Id { get; set; }

    public string Baslik { get; set; }

    public string Model { get; set; }

    public decimal Fiyat { get; set; }

    public int IndirimYuzdesi { get; set; }

    public string KapakUrl { get; set; }

    public string Aciklama { get; set; }

    public string KargoBilgisi { get; set; }

    public string IndirimliFiyat
        => (Fiyat * (100 - IndirimYuzdesi) / 100).ToString("F");
}

public static class UrunExtension
{
    public static IQueryable<UrunVM> AsUrunVM(this IQueryable<Urun> queryable)
        => queryable.Select(u => new UrunVM
        {
            Id = u.Id,
            Baslik = u.Baslik,
            Model = u.Model,
            Fiyat = u.Fiyat,
            IndirimYuzdesi = u.IndirimYuzdesi,
            KapakUrl = u.KapakUrl,
            Aciklama = u.Aciklama,
            KargoBilgisi = u.KargoBilgisi
        });

    public static UrunVM ToUrunVM(this Urun urun)
        => new()
        {
            Id = urun.Id,
            Baslik = urun.Baslik,
            Model = urun.Model,
            Fiyat = urun.Fiyat,
            IndirimYuzdesi = urun.IndirimYuzdesi,
            KapakUrl = urun.KapakUrl,
            Aciklama = urun.Aciklama,
            KargoBilgisi = urun.KargoBilgisi
        };
}

