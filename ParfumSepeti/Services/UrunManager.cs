using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Data;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Services;

public class UrunManager : Manager<Urun>
{
    public static readonly string[] AllowedImageExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp",
        ".gif"
    };

    public const int MAX_UPLOAD_SIZE = 5_000_000;

    private readonly IWebHostEnvironment _env;

    public UrunManager(AppDbContext db, IWebHostEnvironment env) : base(db)
    {
        _env = env;
    }

    public async Task<Result<UrunListeleVM>> GetListeleVMAsync(int page = 1,
                                                               int pageSize = 20)
    {
        if (!await ValidPage(page, pageSize))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz sayfa" }
            };

        var items = await GetQueryable(include: u => u.Kategori,
                                       tracked: false,
                                       pageSize: pageSize,
                                       page: page)
            .Select(u => new UrunListeleItem
            {
                Id = u.Id,
                Baslik = u.Baslik,
                Model = u.Model,
                Fiyat = u.Fiyat,
                Indirim = u.IndirimYuzdesi,
                KapakUrl = u.KapakUrl,
                Kategori = u.Kategori.Isim
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

    public async Task<UrunOlusturVM> GetOlusturVMAsync()
    {
        var selectListItems = await _db.Kategori.Select(k => new SelectListItem
        {
            Value = k.Id.ToString(),
            Text = k.Isim
        })
            .ToListAsync();

        return new()
        {
            Kategoriler = selectListItems
        };
    }

    public async Task PopulateOlusturVMAsync(UrunOlusturVM vm)
    {
        vm.Kategoriler = await _db.Kategori.Select(k => new SelectListItem
        {
            Value = k.Id.ToString(),
            Text = k.Isim
        })
            .ToListAsync();
    }

    public async Task<Result> CreateAsync(UrunOlusturVM vm, IFormFile? file)
    {
        if (file == null)
            return new()
            {
                Success = false,
                Errors = { "Kapak resmi gerekli" }
            };


        if (file.Length > MAX_UPLOAD_SIZE)
            return new()
            {
                Success = false,
                Errors = { "Resim 5MB'tan büyük olamaz" }
            };

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!AllowedImageExtensions.Contains(extension))
            return new()
            {
                Success = false,
                Errors = { $"Geçerli uzantılar: {string.Join(", ", AllowedImageExtensions)}" }
            };

        var imageName = $"{Guid.NewGuid()}{extension}";
        var kapakUrl = $"resimler/urunler/{imageName}";

        var urun = new Urun
        {
            Baslik = vm.Baslik,
            Model = vm.Model,
            KategoriId = vm.KategoriId,
            Fiyat = vm.Fiyat,
            IndirimYuzdesi = vm.IndirimYuzdesi ?? 0,
            Aciklama = vm.Aciklama,
            KargoBilgisi = vm.KargoBilgisi,
            KapakUrl = kapakUrl
        };

        await AddAsync(urun);

        var dir = Path.Combine(_env.WebRootPath, "resimler", "urunler");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var imagePath = Path.Combine(dir, imageName);

        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return new();
    }

    public async Task<Result<UrunSilVM>> GetSilVMAsync(int id)
    {
        var urun = await _set
            .Include(u => u.Kategori)
            .Include(u => u.SepetOgeleri)
            .Include(u => u.Isteyenler)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (urun == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz ürün" }
            };

        return new()
        {
            Object = new()
            {
                Id = urun.Id,
                Baslik = urun.Baslik,
                Model = urun.Model,
                Kategori = urun.Kategori.Isim,
                KapakUrl = urun.KapakUrl,
                IstekSayisi = urun.Isteyenler.Count,
                SepetSayisi = urun.SepetOgeleri.Count
            }
        };
    }

    public async Task<Result> RemoveAsync(UrunSilVM vm)
    {
        var urun = await GetFirstOrDefaultAsync(u => u.Id == vm.Id);

        if (urun == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz ürün" }
            };

        var imagePath = Path.Combine(_env.WebRootPath, urun.KapakUrl);

        if (File.Exists(imagePath))
            File.Delete(imagePath);
            
        Remove(urun);

        return new();
    }
}
