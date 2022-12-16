using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Data;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Services;

public class KategoriManager : Manager<Kategori>
{
    public KategoriManager(AppDbContext db) : base(db)
    {
    }

    public async Task<KategoriListeleVM> GetListeleVMAsync(int page = 1,
                                                           int pageSize = 20)
    {
        var model = new KategoriListeleVM();

        var items = await GetQueryable(include: k => k.Urunler,
                                       tracked: false,
                                       pageSize: pageSize,
                                       page: page)
            .Select(k => new KategoriListeleItem
            {
                Id = k.Id,
                Isim = k.Isim,
                UrunSayisi = k.Urunler.Count
            })
            .ToListAsync();

        return new()
        {
            CurrentPage = page,
            Items = items,
            PageSize = pageSize
        };
    }

    public async Task CreateAsync(KategoriOlusturVM model)
    {
        var kategori = new Kategori
        {
            Isim = model.Isim
        };

        await AddAsync(kategori);
    }

    public async Task<Result<KategoriSilVM>> GetSilVMAsync(int id)
    {
        var kategori = await GetFirstOrDefaultAsync(filter: k => k.Id == id,
                                                    include: k => k.Urunler,
                                                    tracked: false);

        if (kategori == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kategori" }
            };

        var model = new KategoriSilVM
        {
            Id = id,
            Isim = kategori.Isim,
            UrunSayisi = kategori.Urunler.Count
        };

        return new()
        {
            Object = model
        };
    }

    public async Task<Result> RemoveAsync(KategoriSilVM model)
    {
        var kategori = await GetFirstOrDefaultAsync(k => k.Id == model.Id);

        if (kategori == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kategori" }
            };

        Remove(kategori);

        return new();
    }

    public async Task<Result<KategoriDuzenleVM>> GetDuzenleVMAsync(int id)
    {
        var kategori = await GetFirstOrDefaultAsync(k => k.Id == id, false);

        if (kategori == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kategori" }
            };

        var model = new KategoriDuzenleVM
        {
            Id = kategori.Id,
            Isim = kategori.Isim
        };

        return new()
        {
            Object = model
        };
    }

    public async Task<Result> UpdateAsync(KategoriDuzenleVM model)
    {
        var kategori = await GetFirstOrDefaultAsync(k => k.Id == model.Id);

        if (kategori == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kategori" }
            };

        if (await AnyAsync(k => k.Id != model.Id && k.Isim == model.Isim))
            return new()
            {
                Success = false,
                Errors = { $"'{model.Isim}' zaten mevcut" }
            };

        kategori.Isim = model.Isim;

        return new();
    }
}
