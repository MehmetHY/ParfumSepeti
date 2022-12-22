using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Const;
using ParfumSepeti.Data;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Services;

public class SiparisManager : Manager<Siparis>
{
    public SiparisManager(AppDbContext db) : base(db)
    {
    }

    public async Task<AdminSiparisListeleVM> GetSiparisListeleVMAsync()
    {
        var siparislerDb = await _set
            .AsNoTracking()
            .Include(s => s.Ogeler)
            .Include(s => s.Kullanici)
            .OrderByDescending(s => s.OlusturmaTarihi)
            .ToListAsync();

        var siparisler = new List<AdminSiparisListeleVM.Siparis>();
        var onemliSiparisler = new List<AdminSiparisListeleVM.Siparis>();

        foreach (var siparisDb in siparislerDb)
        {
            var siparis = new AdminSiparisListeleVM.Siparis
            {
                Id = siparisDb.Id,
                Kullanici = siparisDb.Kullanici?.UserName ?? "-",
                KargoDurumu = siparisDb.KargoDurumu,
                OdemeDurumu = siparisDb.OdemeDurumu,
                OlusuturmaTarihi = siparisDb.OlusturmaTarihi,

                Ogeler = siparisDb.Ogeler
                    .Select(o => new AdminSiparisListeleVM.Siparis.Oge
                    {
                        UrunIsmi = o.UrunIsmi,
                        Fiyat = o.Fiyat,
                        Adet = o.Adet
                    })
                    .ToList(),

                Toplam = siparisDb.Ogeler
                    .Reduce<SiparisOgesi, decimal>((o, t) => t + o.Fiyat * o.Adet)
                    .ToString("F2")
            };

            if (siparisDb.OdemeDurumu == OdemeDurumu.ODENDI &&
                siparisDb.KargoDurumu == KargoDurumu.GONDERILMEDI)
            {
                onemliSiparisler.Add(siparis);
            }
            else
            {
                siparisler.Add(siparis);
            }
        }

        onemliSiparisler.AddRange(siparisler);

        return new()
        {
            Siparisler = onemliSiparisler
        };
    }

    public async Task<Result<AdminSiparisDetayVM>> GetSiparisDetayVMAsync(int siparisId)
    {
        var siparis = await _set
            .AsNoTracking()
            .Include(s => s.Ogeler)
            .Include(s => s.Kullanici)
            .FirstOrDefaultAsync(s => s.Id == siparisId);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz siparis" }
            };

        var ogeler = siparis.Ogeler
            .Select(o => new AdminSiparisDetayVM.Oge
            {
                Urun = o.UrunIsmi,
                Fiyat = o.Fiyat,
                Adet = o.Adet
            })
            .ToList();

        var toplam = siparis.Ogeler.Reduce<SiparisOgesi, decimal>(
            (o, t) => t + o.Fiyat * o.Adet
        );

        return new()
        {
            Object = new()
            {
                SiparisId = siparis.Id,
                Kullanici = siparis.Kullanici?.UserName ?? "-",
                Ogeler = ogeler,
                Toplam = toplam.ToString("F2"),
                OlusturmaTarihi = siparis.OlusturmaTarihi.ToString(),
                OdemeDurumu = siparis.OdemeDurumu,
                OdemeTarihi = siparis.OdemeTarihi?.ToString() ?? "-",
                KargoDurumu = siparis.KargoDurumu,
                KargoyaVerilmeTarihi = siparis.KargoyaVerilmeTarihi?.ToString() ?? "-",
                KargoFirmasi = siparis.KargoSirketi ?? "-",
                KargoTakipKodu = siparis.KargoTakipKodu ?? "-",
                Isim = siparis.Isim ?? "-",
                SoyIsim = siparis.SoyIsim ?? "-",
                Telefon = siparis.Telefon ?? "-",
                Il = siparis.Il ?? "-",
                Ilce = siparis.Ilce,
                Adres = siparis.Adres ?? "-",
                PostaKodu = siparis.PostaKodu ?? "-"
            }
        };
    }
}
