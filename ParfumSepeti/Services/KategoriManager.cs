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

    public async Task<Result<KategoriSilVM>> GetSilVMAsync(string isim)
    {
        var kategori = await GetFirstOrDefaultAsync(filter: k => k.Isim == isim,
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
        var kategori = await GetFirstOrDefaultAsync(
            k => k.Isim == (model.Isim ?? string.Empty)
        );

        if (kategori == null)
            return new Result
            {
                Success = false,
                Errors = { "Geçersiz kategori" }
            };

        Remove(kategori);

        return new();
    }
}
