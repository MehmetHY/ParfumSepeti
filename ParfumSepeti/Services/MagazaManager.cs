using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Const;
using ParfumSepeti.Data;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;
using Stripe.Checkout;

namespace ParfumSepeti.Services;

public class MagazaManager
{
    private readonly AppDbContext _db;
    private readonly KategoriManager _kategoriManager;
    private readonly UrunManager _urunManager;
    private readonly KullaniciManager _kullaniciManager;

    public MagazaManager(AppDbContext db,
                         KategoriManager kategoriManager,
                         UrunManager urunManager,
                         KullaniciManager kullaniciManager)
    {
        _db = db;
        _kategoriManager = kategoriManager;
        _urunManager = urunManager;
        _kullaniciManager = kullaniciManager;
    }

    public async Task<AnasayfaVM> GetAnasayfaVMAsync()
    {
        var yeniUrunlerDb = await _urunManager.GetYeniEklenenlerAsync(8);
        var indirimliUrunlerDb = await _urunManager.GetIndirimdekilerAsync(8);
        var kategorilerDb = await _kategoriManager.GetAllAsync();

        var kategoriler = new List<AnasayfaVM.Kategori>();

        foreach (var kategori in kategorilerDb)
        {
            var urunler = await _urunManager.GetOfKategori(kategori.Id, 8);

            var item = new AnasayfaVM.Kategori
            {
                Id = kategori.Id,
                Isim = kategori.Isim,

                Urunler = urunler
                    .Select(u => new UrunCardVM
                    {
                        Id = u.Id,
                        Baslik = u.Baslik,
                        Model = u.Model,
                        Kategori = kategori.Isim,
                        Fiyat = u.Fiyat,
                        Indirim = u.IndirimYuzdesi,
                        KapakUrl = u.KapakUrl
                    })
                    .ToList()
            };

            kategoriler.Add(item);
        }

        var vm = new AnasayfaVM
        {
            YeniEklenenler = yeniUrunlerDb
                .Select(u => new UrunCardVM
                {
                    Id = u.Id,
                    Baslik = u.Baslik,
                    Model = u.Model,
                    Kategori = u.Kategori.Isim,
                    Fiyat = u.Fiyat,
                    Indirim = u.IndirimYuzdesi,
                    KapakUrl = u.KapakUrl
                })
                .ToList(),

            Indirimler = indirimliUrunlerDb
                .Select(u => new UrunCardVM
                {
                    Id = u.Id,
                    Baslik = u.Baslik,
                    Model = u.Model,
                    Kategori = u.Kategori.Isim,
                    Fiyat = u.Fiyat,
                    Indirim = u.IndirimYuzdesi,
                    KapakUrl = u.KapakUrl
                })
                .ToList(),

            Kategoriler = kategoriler
        };

        return vm;
    }

    public async Task<Result<KategoriVM>> GetKategoriVMAsync(int id,
                                                             int page = 1,
                                                             int pageSize = 20)
    {
        var kategori = await _kategoriManager.GetFirstOrDefaultAsync(k => k.Id == id,
                                                                     false);

        if (kategori == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kategori" }
            };

        var query = _db.Urun
            .AsNoTracking()
            .Where(u => u.KategoriId == kategori.Id);

        if (!await query.ValidPageAsync(page, pageSize))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz sayfa" }
            };

        var lastPage = await query.PageCountAsync(pageSize);

        var cards = await query
            .OrderByDescending(u => u.EklenmeTarihi)
            .Page(page, pageSize)
            .Include(u => u.Kategori)
            .AsUrunCardVMs()
            .ToListAsync();

        return new()
        {
            Object = new()
            {
                Id = id,
                Isim = kategori.Isim,
                Items = cards,
                CurrentPage = page,
                PageSize = pageSize,
                LastPage = lastPage
            }
        };
    }

    public async Task<Result<IndirimVM>> GetIndirimVMAsync(int page = 1,
                                                           int pageSize = 20)
    {
        var query = _db.Urun
            .AsNoTracking()
            .Where(u => u.IndirimYuzdesi > 0);

        if (!await query.ValidPageAsync(page, pageSize))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz sayfa" }
            };

        var lastPage = await query.PageCountAsync(pageSize);

        var cards = await query
            .OrderByDescending(u => u.EklenmeTarihi)
            .Page(page, pageSize)
            .Include(u => u.Kategori)
            .AsUrunCardVMs()
            .ToListAsync();

        return new()
        {
            Object = new()
            {
                Items = cards,
                CurrentPage = page,
                PageSize = pageSize,
                LastPage = lastPage
            }
        };
    }

    public async Task<Result<YeniVM>> GetYeniVMAsync(int page = 1,
                                                     int pageSize = 20)
    {
        var query = _db.Urun
            .AsNoTracking()
            .OrderByDescending(u => u.EklenmeTarihi);

        if (!await query.ValidPageAsync(page, pageSize))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz sayfa" }
            };

        var lastPage = await query.PageCountAsync(pageSize);

        var cards = await query
            .Page(page, pageSize)
            .Include(u => u.Kategori)
            .AsUrunCardVMs()
            .ToListAsync();

        return new()
        {
            Object = new()
            {
                Items = cards,
                CurrentPage = page,
                PageSize = pageSize,
                LastPage = lastPage
            }
        };
    }

    public async Task<Result<AramaVM>> GetAramaVMAsync(string metin,
                                                       int page = 1,
                                                       int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(metin))
            return new()
            {
                Object = new()
            };

        var query = _db.Urun
            .AsNoTracking()
            .Where(u => u.Baslik.Contains(metin) || u.Aciklama.Contains(metin));

        if (!await query.ValidPageAsync(page, pageSize))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz sayfa" }
            };

        var lastPage = await query.PageCountAsync(pageSize);

        var cards = await query
            .Page(page, pageSize)
            .OrderByDescending(u => u.EklenmeTarihi)
            .AsUrunCardVMs()
            .ToListAsync();

        return new()
        {
            Object = new()
            {
                Items = cards,
                AramaMetni = metin,
                CurrentPage = page,
                PageSize = pageSize,
                LastPage = lastPage
            }
        };
    }

    public async Task<Result<UrunVM>> GetUrunVMAsync(int id)
    {
        var urun = await _urunManager.GetFirstOrDefaultAsync(u => u.Id == id, false);

        if (urun == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz ürün" }
            };

        return new()
        {
            Object = urun.ToUrunVM()
        };
    }

    public async Task<SepetVM> GetSepetVMAsync(ISession session)
    {
        var sepetSession = session.Get<SepetSessionObject>("sepet") ?? new();
        var items = new List<SepetItem>();

        foreach (var item in sepetSession.Items)
        {
            var urun = await _db.Urun
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == item.UrunId);

            if (urun == null)
                continue;

            items.Add(new()
            {
                Adet = item.Adet,
                Id = urun.Id,
                Baslik = urun.Baslik,
                Model = urun.Model,
                KapakUrl = urun.KapakUrl,
                BirimFiyat = urun.Fiyat * (100 - urun.IndirimYuzdesi) / 100
            });
        }

        var toplam = items.Reduce<SepetItem, decimal>(
            (i, t) => t + i.Adet * i.BirimFiyat
        );

        return new()
        {
            Items = items,
            Toplam = toplam.ToString("F2")
        };
    }

    public async Task<Result> SepeteEkleAsync(ISession session, int id, int adet)
    {
        var sepetSession = session.Get<SepetSessionObject>("sepet") ?? new();
        var item = sepetSession.Items.FirstOrDefault(i => i.UrunId == id);

        adet = Math.Clamp(adet, 1, 10);

        if (item != null)
        {
            item.Adet = adet;
            session.Set("sepet", sepetSession);

            return new();
        }

        var urun = await _db.Urun
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (urun == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz ürün" }
            };

        sepetSession.Items.Add(new()
        {
            Adet = adet,
            UrunId = id
        });

        session.Set("sepet", sepetSession);

        return new();
    }

    public void SepettenKaldir(ISession session, int id)
    {
        var sepetSession = session.Get<SepetSessionObject>("sepet");

        if (sepetSession == null || !sepetSession.Items.Any(i => i.UrunId == id))
            return;

        var item = sepetSession.Items.FirstOrDefault(i => i.UrunId == id);
        sepetSession.Items.Remove(item!);
        session.Set("sepet", sepetSession);
    }

    public bool SepetteMi(ISession session, int id)
    {
        var sepetSession = session.Get<SepetSessionObject>("sepet");

        return sepetSession != null && sepetSession.Items.Any(i => i.UrunId == id);
    }

    public async Task<Result<SiparisOlusturVM>> SiparisOlusturAsync(
        ISession session,
        SiparisBilgiVM vm,
        string? kullaniciAdi,
        string domain)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var kullanici = await _kullaniciManager.Set
            .Include(k => k.Siparisler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz Kullanıcı" }
            };

        var sepetSession = session.Get<SepetSessionObject>("sepet");

        if (sepetSession == null || sepetSession.Items.Count < 1)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Sepet bulunamadı" }
            };

        var ogeler = new List<SiparisOgesi>();

        var stripeOptions = new SessionCreateOptions
        {
            LineItems = new(),
            PaymentMethodTypes = new() { "card" },
            Mode = "payment",
            CancelUrl = $"{domain}/Kullanici/Siparisler"
        };

        foreach (var item in sepetSession.Items)
        {
            var urun = await _db.Urun
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == item.UrunId);

            if (urun == null)
                return new()
                {
                    Success = false,
                    Fatal = true,
                    Errors = { "Geçersiz ürün" }
                };

            var oge = new SiparisOgesi
            {
                UrunId = urun.Id,
                UrunIsmi = $"{urun.Baslik} {urun.Model}",
                Adet = item.Adet,
                Fiyat = urun.Fiyat * (100 - urun.IndirimYuzdesi) / 100
            };

            ogeler.Add(oge);

            var stripeLineOption = new SessionLineItemOptions
            {
                PriceData = new()
                {
                    UnitAmount = (int)(oge.Fiyat * 100),
                    Currency = "try",
                    ProductData = new()
                    {
                        Name = oge.UrunIsmi
                    }
                },
                Quantity = oge.Adet
            };

            stripeOptions.LineItems.Add(stripeLineOption);
        }

        if (ogeler.Count < 1)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz sepet" }
            };

        var siparis = new Siparis
        {
            KullaniciId = kullanici.Id,
            Ogeler = ogeler,
            OdemeDurumu = OdemeDurumu.ODENMEDI,
            KargoDurumu = KargoDurumu.GONDERILMEDI,
            OlusturmaTarihi = DateTime.Now,
            Isim = vm.Isim,
            SoyIsim = vm.SoyIsim,
            Telefon = vm.Telefon,
            Il = vm.Il,
            Ilce = vm.Ilce,
            Adres = vm.Adres,
            PostaKodu = vm.PostaKodu
        };

        await _db.Siparis.AddAsync(siparis);
        await _db.SaveChangesAsync();

        stripeOptions.SuccessUrl =
            $"{domain}/Magaza/SiparisOnayla?siparisId={siparis.Id}";

        var stripeService = new SessionService();
        var stripeSession = await stripeService.CreateAsync(stripeOptions);

        siparis.SessionId = stripeSession.Id;
        siparis.OdemeIntentId = stripeSession.PaymentIntentId;
        await _db.SaveChangesAsync();

        return new()
        {
            Object = new()
            {
                SiparisId = siparis.Id,
                OdemeUrl = stripeSession.Url
            }
        };
    }

    public bool SepetValid(ISession session)
    {
        var sepet = session.Get<SepetSessionObject>("sepet");

        return sepet != null && sepet.Items.Count > 0;
    }

    public async Task<Result> SiparisiOnaylaAsync(ISession session,
                                                  string? kullaniciAdi,
                                                  int siparisId)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await _kullaniciManager.Set
            .Include(k => k.Siparisler)
            .FirstOrDefaultAsync(k => k.UserName == kullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        var siparis = kullanici.Siparisler.FirstOrDefault(s => s.Id == siparisId);

        if (siparis == null)
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz sipariş" }
            };

        var stripeService = new SessionService();
        var stripeSession = await stripeService.GetAsync(siparis.SessionId);

        if (stripeSession.PaymentStatus.ToLower() != "paid")
            return new()
            {
                Success = false,
                Fatal = false,
                Errors = { "Ödeme yapılamadı" }
            };

        siparis.OdemeDurumu = OdemeDurumu.ODENDI;
        siparis.OdemeTarihi = DateTime.Now;

        await _db.SaveChangesAsync();
        SepetiTemizle(session);

        return new();
    }

    public void SepetiTemizle(ISession session)
    {
        session.Set("sepet", new SepetSessionObject());
    }
}
