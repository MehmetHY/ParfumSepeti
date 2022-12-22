using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Const;
using ParfumSepeti.Data;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;
using Stripe.Checkout;

namespace ParfumSepeti.Services;

public class KullaniciManager : Manager<Kullanici>
{
    private readonly UserManager<Kullanici> _userManager;

    public KullaniciManager(AppDbContext db,
                            UserManager<Kullanici> userManager) : base(db)
    {
        _userManager = userManager;
    }

    public async Task<Result<KullaniciListeleVM>> GetListeleVMAsync(int page = 1,
                                                                    int pageSize = 20)
    {
        if (!await ValidPage(page, pageSize))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz sayfa" }
            };

        var admins = await _userManager.GetUsersInRoleAsync("admin");
        var adminIds = admins.Select(a => a.Id);

        var items = await GetQueryable(filter: u => !adminIds.Contains(u.Id),
                                       tracked: false,
                                       include: u => u.Siparisler,
                                       page: page,
                                       pageSize: pageSize)
            .Select(u => new KullaniciListeleItem
            {
                Id = u.Id,
                Isim = u.UserName ?? "-",

                OdenmisSiparis = u.Siparisler
                    .Where(s => s.OdemeDurumu == OdemeDurumu.ODENDI)
                    .Count()
            })
            .ToListAsync();

        return new()
        {
            Object = new()
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize
            }
        };
    }

    public async Task<Result<KullaniciSilVM>> GetSilVMAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await GetFirstOrDefaultAsync(filter: k => k.Id == id,
                                                     include: k => k.Siparisler,
                                                     tracked: false);

        if (kullanici == null || await _userManager.IsInRoleAsync(kullanici, "admin"))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kullanıcı" }
            };

        return new()
        {
            Object = new()
            {
                Id = kullanici.Id,
                Isim = kullanici.UserName ?? "-",

                OdenenSiparis = kullanici.Siparisler
                    .Where(s => s.OdemeDurumu == OdemeDurumu.ODENDI)
                    .Count()
            }
        };
    }

    public async Task<Result> RemoveAsync(KullaniciSilVM vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Id))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await _userManager.FindByIdAsync(vm.Id);

        if (kullanici == null || await _userManager.IsInRoleAsync(kullanici, "admin"))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kullanıcı" }
            };

        var result = await _userManager.DeleteAsync(kullanici);

        if (!result.Succeeded)
            return new()
            {
                Success = false,
                Errors = { "Silinemedi" }
            };

        return new();
    }

    public async Task<Result<KullaniciDuzenleVM>> GetDuzenleVMAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await GetFirstOrDefaultAsync(filter: k => k.Id == id,
                                                     tracked: false);

        if (kullanici == null || await _userManager.IsInRoleAsync(kullanici, "admin"))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        return new()
        {
            Object = new()
            {
                Id = kullanici.Id,
                KullaniciAdi = kullanici.UserName ?? "-"
            }
        };
    }

    public async Task<Result> UpdateAsync(KullaniciDuzenleVM vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Id))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await GetFirstOrDefaultAsync(filter: k => k.Id == vm.Id);

        if (kullanici == null || await _userManager.IsInRoleAsync(kullanici, "admin"))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        if (await AnyAsync(k => k.Id != kullanici.Id && k.UserName == vm.KullaniciAdi))
            return new()
            {
                Success = false,
                Errors = { $"Kullanıcı adı '{vm.KullaniciAdi}' zaten mevcut" }
            };

        kullanici.UserName = vm.KullaniciAdi;

        return new();
    }

    public async Task<Result<IstekListesiVM>> GetIstekListesiVMAsync(string? isim,
                                                                     int page = 1,
                                                                     int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(isim))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçeriz kullanıcı" }
            };

        var kullanici = await _set
            .AsNoTracking()
            .Where(k => k.UserName == isim)
            .Include(k => k.IstekListesi)
            .ThenInclude(u => u.Kategori)
            .FirstOrDefaultAsync();


        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçeriz kullanıcı" }
            };

        if (!kullanici.IstekListesi.ValidPage(page, pageSize))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçeriz sayfa" }
            };

        var cards = kullanici.IstekListesi
            .Page(page, pageSize)
            .AsUrunCardVMs();

        return new()
        {
            Object = new()
            {
                Items = cards,
                CurrentPage = page,
                PageSize = pageSize
            }
        };
    }

    public async Task<bool> IstekListesindeMi(string? kullaniciAdi, int urunId)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return false;

        var kullanici = await _set
            .Include(k => k.IstekListesi)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return false;

        return kullanici.IstekListesi.Any(u => u.Id == urunId);
    }

    public async Task IstekListesineEkle(string? kullaniciAdi, int urunId)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return;

        var kullanici = await _set
            .Include(k => k.IstekListesi)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return;

        if (kullanici.IstekListesi.Any(u => u.Id == urunId))
            return;

        var urun = await _db.Urun
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == urunId);

        if (urun == null)
            return;

        kullanici.IstekListesi.Add(urun);
        await _db.SaveChangesAsync();
    }

    public async Task IstekListesindenKaldir(string? kullaniciAdi, int urunId)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return;

        var kullanici = await _set
            .Include(k => k.IstekListesi)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return;

        var urun = kullanici.IstekListesi.FirstOrDefault(u => u.Id == urunId);

        if (urun == null)
            return;

        kullanici.IstekListesi.Remove(urun);
        await _db.SaveChangesAsync();
    }

    public async Task<Result<KullaniciSiparisleriVM>> GetSiparislerVM(string? kullaniciAdi)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçeriz kullanıcı" }
            };

        var kullanici = await _set
            .AsNoTracking()
            .Include(k => k.Siparisler)
            .ThenInclude(s => s.Ogeler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçeriz kullanıcı" }
            };

        var vm = new KullaniciSiparisleriVM();

        foreach (var siparis in kullanici.Siparisler)
        {
            var ogeler = siparis.Ogeler
                .Select(o => new KullaniciSiparisleriVM.SiparisItem.Oge
                {
                    Adet = o.Adet,
                    Fiyat = o.Fiyat,
                    UrunIsmi = o.UrunIsmi
                })
                .ToList();

            var item = new KullaniciSiparisleriVM.SiparisItem
            {
                Id = siparis.Id,
                Ogeler = ogeler,
                Toplam = ogeler.Reduce<KullaniciSiparisleriVM.SiparisItem.Oge, decimal>(
                    (oge, toplam) => toplam + oge.Fiyat * oge.Adet
                ),
                KargoDurumu = siparis.KargoDurumu,
                OdemeDurumu = siparis.OdemeDurumu,
                OlusuturmaTarihi = siparis.OlusturmaTarihi
            };

            vm.Items.Add(item);
        }

        return new()
        {
            Object = vm
        };
    }

    public async Task<Result<SiparisDetayVM>> GetSiparisDetayVMAsync(string? kullaniciAdi,
                                                                     int siparisId)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var kullanici = await _set
            .Include(k => k.Siparisler)
            .ThenInclude(s => s.Ogeler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var siparis = kullanici.Siparisler.FirstOrDefault(s => s.Id == siparisId);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Sipariş" }
            };

        var ogeler = siparis.Ogeler
            .Select(o => new SiparisDetayVM.Oge
            {
                Adet = o.Adet,
                Fiyat = o.Fiyat,
                Urun = o.UrunIsmi
            })
            .ToList();

        var toplam = ogeler.Reduce<SiparisDetayVM.Oge, decimal>(
            (o, t) => t + o.Fiyat * o.Adet
        );

        var detay = new SiparisDetayVM
        {
            SiparisId = siparis.Id,
            Ogeler = ogeler,
            Toplam = toplam.ToString("F2"),

            OlusturmaTarihi = siparis.OlusturmaTarihi.ToString(),
            OdemeDurumu = siparis.OdemeDurumu,
            OdemeTarihi = siparis.OdemeTarihi?.ToString() ?? "-",

            KargoDurumu = siparis.KargoDurumu,
            KargoyaVerilmeTarihi = siparis.KargoyaVerilmeTarihi?.ToString() ?? "-",
            KargoFirmasii = siparis.KargoSirketi ?? "-",
            KargoTakipKodu = siparis.KargoTakipKodu ?? "-",

            Isim = siparis.Isim ?? "-",
            SoyIsim = siparis.SoyIsim ?? "-",
            Telefon = siparis.Telefon ?? "-",
            Il = siparis.Il ?? "-",
            Ilce = siparis.Ilce ?? "-",
            Adres = siparis.Adres ?? "-",
            PostaKodu = siparis.PostaKodu ?? "-"
        };

        return new()
        {
            Object = detay
        };
    }

    public async Task<Result<SiparisAdresDuzenleVM>> GetSiparisAdresDuzenleVMAsync(
        string? kullaniciAdi,
        int siparisId
    )
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var kullanici = await _set
            .Include(k => k.Siparisler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var siparis = kullanici.Siparisler.FirstOrDefault(s => s.Id == siparisId);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Sipariş" }
            };

        return new()
        {
            Object = new()
            {
                Id = siparis.Id,
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

    public async Task<Result> SiparisAdresGuncelleAsync(string? kullaniciAdi,
                                                        SiparisAdresDuzenleVM vm)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var kullanici = await _set
            .Include(k => k.Siparisler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var siparis = kullanici.Siparisler.FirstOrDefault(s => s.Id == vm.Id);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Sipariş" }
            };

        siparis.Isim = vm.Isim;
        siparis.SoyIsim = vm.SoyIsim;
        siparis.Telefon = vm.Telefon;
        siparis.Il = vm.Il;
        siparis.Ilce = vm.Ilce;
        siparis.Adres = vm.Adres;
        siparis.PostaKodu = vm.PostaKodu;

        await _db.SaveChangesAsync();

        return new();
    }

    public async Task<Result<SiparisIptalVM>> GetSiparisIptalVMAsync(string? kullaniciAdi,
                                                                     int siparisId)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var kullanici = await _set
            .Include(k => k.Siparisler)
            .ThenInclude(s => s.Ogeler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var siparis = kullanici.Siparisler.FirstOrDefault(s => s.Id == siparisId);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Sipariş" }
            };

        var ogeler = siparis.Ogeler
            .Select(o => new SiparisIptalVM.Oge
            {
                Urun = o.UrunIsmi,
                Fiyat = o.Fiyat,
                Adet = o.Adet
            })
            .ToList();

        var toplam = ogeler.Reduce<SiparisIptalVM.Oge, decimal>(
            (o, t) => t + o.Fiyat * o.Adet
        );

        return new()
        {
            Object = new()
            {
                Id = siparis.Id,
                Ogeler = ogeler,
                Toplam = toplam.ToString("F2"),
                OlusturmaTarihi = siparis.OlusturmaTarihi.ToString()
            }
        };
    }

    public async Task<Result> SiparisIptalEtAsync(string? kullaniciAdi, SiparisIptalVM vm)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var kullanici = await _set
            .Include(k => k.Siparisler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var siparis = kullanici.Siparisler.FirstOrDefault(s => s.Id == vm.Id);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Sipariş" }
            };

        kullanici.Siparisler.Remove(siparis);
        await _db.SaveChangesAsync();

        return new();
    }

    public async Task<Result<string>> GetSiparisOdemeUrlAsync(string? kullaniciAdi,
                                                              int siparisId)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var kullanici = await _set
            .Include(k => k.Siparisler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var siparis = kullanici.Siparisler.FirstOrDefault(s => s.Id == siparisId);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Sipariş" }
            };

        if (siparis.OdemeDurumu == OdemeDurumu.ODENDI)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Sipariş zaten ödendi" }
            };

        var stripeService = new SessionService();
        var stripeSession = await stripeService.GetAsync(siparis.SessionId);

        if (stripeSession == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Servis hatası ya da geçersiz sipariş" }
            };

        return new()
        {
            Object = stripeSession.Url
        };
    }
}
