using ParfumSepeti.Models;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

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

public static class UrunCardVMExtension
{
    public static IQueryable<UrunCardVM> AsUrunCardVMs(this IQueryable<Urun> queryable)
        => queryable.Select(u => new UrunCardVM
        {
            Id = u.Id,
            Baslik = u.Baslik,
            Model = u.Model,
            Fiyat = u.Fiyat,
            Indirim = u.IndirimYuzdesi,
            KapakUrl = u.KapakUrl,
            Kategori = u.Kategori.Isim
        });

    public static List<UrunCardVM> AsUrunCardVMs(this IEnumerable<Urun> urunler)
        => urunler.Select(u => new UrunCardVM
        {
            Id = u.Id,
            Baslik = u.Baslik,
            Model = u.Model,
            Fiyat = u.Fiyat,
            Indirim = u.IndirimYuzdesi,
            KapakUrl = u.KapakUrl,
            Kategori = u.Kategori.Isim
        })
        .ToList();
}