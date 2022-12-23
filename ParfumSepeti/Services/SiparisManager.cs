using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<Result<AdminSiparisListeleVM>> GetListeleVMAsync(int page = 1,
                                                               int pageSize = 20)
    {
        var query = _set.AsNoTracking();

        if (!await query.ValidPageAsync(page, pageSize))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçeriz sayfa" }
            };

        var lastPage = await query.PageCountAsync(pageSize);

        var siparislerDb = await query
            .AsNoTracking()
            .Include(s => s.Ogeler)
            .Include(s => s.Kullanici)
            .ToListAsync();

        siparislerDb = siparislerDb
            .OrderBy(s => s.OdemeDurumu, new SiparisOdemeComparer())
            .ThenBy(s => s.KargoDurumu, new SiparisKargoComparer())
            .ThenByDescending(s => s.OlusturmaTarihi)
            .Page(page, pageSize)
            .ToList();

        var siparisler = new List<AdminSiparisListeleVM.Siparis>();

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

            siparisler.Add(siparis);
        }

        return new()
        {
            Object = new()
            {
                Items = siparisler,
                CurrentPage = page,
                PageSize = pageSize,
                LastPage = lastPage
            }
        };
    }

    public async Task<Result<AdminSiparisDetayVM>> GetDetayVMAsync(int siparisId)
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
                Ilce = siparis.Ilce ?? "-",
                Adres = siparis.Adres ?? "-",
                PostaKodu = siparis.PostaKodu ?? "-"
            }
        };
    }

    public async Task<Result<AdminSiparisSilVM>> GetSilVMAsync(int siparisId)
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
            .Select(o => new AdminSiparisSilVM.Oge
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
                Id = siparis.Id,
                Kullanici = siparis.Kullanici?.UserName ?? "-",
                OlusturmaTarihi = siparis.OlusturmaTarihi.ToString(),
                Ogeler = ogeler,
                Toplam = toplam.ToString("F2"),
                OdemeDurumu = siparis.OdemeDurumu,
                KargoDurumu = siparis.KargoDurumu
            }
        };
    }

    public async Task<Result> SilAsync(int siparisId)
    {
        var siparis = await _set.FirstOrDefaultAsync(s => s.Id == siparisId);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz siparis" }
            };

        _set.Remove(siparis);
        await _db.SaveChangesAsync();

        return new();
    }

    public List<SelectListItem> GetKargoDurumlari()
    {
        return new()
        {
            new(KargoDurumu.GONDERILDI, KargoDurumu.GONDERILDI),
            new(KargoDurumu.GONDERILMEDI, KargoDurumu.GONDERILMEDI)
        };
    }

    public async Task<Result<AdminSiparisDuzenleVM>> GetDuzenleVMAsync(int siparisId)
    {
        var siparis = await _set
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == siparisId);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz siparis" }
            };

        return new()
        {
            Object = new()
            {
                Id = siparis.Id,
                KargoDurumu = siparis.KargoDurumu,
                KargoDurumlari = GetKargoDurumlari(),
                KargoTakipNo = siparis.KargoTakipKodu,
                TasiyiciFirma = siparis.KargoSirketi
            }
        };
    }

    public async Task<Result> DuzenleAsync(AdminSiparisDuzenleVM vm)
    {
        var siparis = await _set.FirstOrDefaultAsync(s => s.Id == vm.Id);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz siparis" }
            };

        if (vm.KargoDurumu == KargoDurumu.GONDERILMEDI)
        {
            siparis.KargoSirketi = null;
            siparis.KargoTakipKodu = null;
            siparis.KargoyaVerilmeTarihi = null;
        }
        else if (vm.KargoDurumu == KargoDurumu.GONDERILDI)
        {
            if (string.IsNullOrWhiteSpace(vm.TasiyiciFirma))
                return new()
                {
                    Success = false,

                    Errors =
                    {
                        "Eğer kargo gönderildiyse, taşıyıcı firma boş bırakılamaz."
                    }
                };

            if (string.IsNullOrWhiteSpace(vm.KargoTakipNo))
                return new()
                {
                    Success = false,

                    Errors =
                    {
                        "Eğer kargo gönderildiyse, kargo takip numarası boş bırakılamaz."
                    }
                };

            siparis.KargoSirketi = vm.TasiyiciFirma;
            siparis.KargoTakipKodu = vm.KargoTakipNo;
            siparis.KargoyaVerilmeTarihi = DateTime.Now;
        }
        else
        {
            return new()
            {
                Success = false,
                Errors = { "Kargo durumu boş bırakılamaz." }
            };
        }

        siparis.KargoDurumu = vm.KargoDurumu;
        await _db.SaveChangesAsync();

        return new();
    }
}
