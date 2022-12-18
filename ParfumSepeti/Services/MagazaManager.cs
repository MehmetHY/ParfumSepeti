﻿using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Data;
using ParfumSepeti.ViewModels;

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

        var result = await _urunManager.GetKategoriUrunCardsAsync(id, page, pageSize);

        if (result.Success)
            return new()
            {
                Object = new()
                {
                    Id = id,
                    Isim = kategori.Isim,
                    Items = result.Object!,
                    CurrentPage = page,
                    PageSize = pageSize
                }
            };

        return new()
        {
            Success = false,
            Fatal = true,
            Errors = result.Errors
        };
    }

    public async Task<Result<IndirimVM>> GetIndirimVMAsync(int page = 1,
                                                           int pageSize = 20)
    {
        var result = await _urunManager.GetIndirimliUrunCardsAsync(page, pageSize);

        if (result.Success)
            return new()
            {
                Object = new()
                {
                    Items = result.Object!,
                    CurrentPage = page,
                    PageSize = pageSize
                }
            };

        return new()
        {
            Success = false,
            Fatal = true,
            Errors = result.Errors
        };
    }

    public async Task<Result<YeniVM>> GetYeniVMAsync(int page = 1,
                                                     int pageSize = 20)
    {
        var result = await _urunManager.GetYeniUrunCardsAsync(page, pageSize);

        if (result.Success)
            return new()
            {
                Object = new()
                {
                    Items = result.Object!,
                    CurrentPage = page,
                    PageSize = pageSize
                }
            };

        return new()
        {
            Success = false,
            Fatal = true,
            Errors = result.Errors
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

        var result = await _urunManager.GetAramaUrunCards(metin, page, pageSize);

        if (!result.Success)
            return new()
            {
                Success = false,
                Fatal = result.Fatal,
                Errors = result.Errors
            };

        return new()
        {
            Object = new()
            {
                AramaMetni = metin,
                Items = result.Object!,
                CurrentPage = page,
                PageSize = pageSize
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
}