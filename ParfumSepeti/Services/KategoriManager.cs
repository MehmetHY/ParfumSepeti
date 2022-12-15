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
}
