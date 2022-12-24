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
        var query = _set.AsNoTracking();

        if (!await query.ValidPageAsync(page, pageSize))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz sayfa" }
            };

        var lastPage = await query.PageCountAsync(pageSize);

        var items = await query
            .OrderByDescending(u => u.EklenmeTarihi)
            .Page(page, pageSize)
            .Include(u => u.Kategori)
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
                PageSize = pageSize,
                LastPage = lastPage
            }
        };
    }

    public async Task<List<SelectListItem>> GetKategoriSelectListAsync()
        => await _db.Kategori.Select(k => new SelectListItem
        {
            Value = k.Id.ToString(),
            Text = k.Isim
        })
            .ToListAsync();

    public async Task<UrunOlusturVM> GetOlusturVMAsync()
        => new()
        {
            Kategoriler = await GetKategoriSelectListAsync()
        };

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
            KapakUrl = kapakUrl,
            EklenmeTarihi = DateTime.Now
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
                IstekSayisi = urun.Isteyenler.Count
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

    public async Task<Result<UrunDuzenleVM>> GetDuzenleVM(int id)
    {
        var urun = await GetFirstOrDefaultAsync(u => u.Id == id, false);

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
                KategoriId = urun.KategoriId,
                Kategoriler = await GetKategoriSelectListAsync(),
                Fiyat = urun.Fiyat,
                IndirimYuzdesi = urun.IndirimYuzdesi,
                Aciklama = urun.Aciklama,
                KargoBilgisi = urun.KargoBilgisi,
                KapakUrl = urun.KapakUrl
            }
        };
    }

    public async Task<Result> UpdateAsync(UrunDuzenleVM vm, IFormFile? file)
    {
        var urun = await GetFirstOrDefaultAsync(u => u.Id == vm.Id);

        if (urun == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz ürün" }
            };

        var newKategori =
            await _db.Kategori.FirstOrDefaultAsync(k => k.Id == vm.KategoriId);

        if (newKategori == null)
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kategori" }
            };

        urun.Baslik = vm.Baslik;
        urun.Model = vm.Model;
        urun.KategoriId = newKategori.Id;
        urun.Fiyat = vm.Fiyat;
        urun.IndirimYuzdesi = vm.IndirimYuzdesi ?? 0;
        urun.Aciklama = vm.Aciklama;
        urun.KargoBilgisi = vm.KargoBilgisi;

        if (vm.KapakDegistir)
        {
            if (file == null)
                return new()
                {
                    Success = false,
                    Errors = { "Kapak resmi değiştirmek için yeni bir resim yükle" }
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

            var newImageName = $"{Guid.NewGuid()}{extension}";
            var newKapakUrl = $"resimler/urunler/{newImageName}";
            var newPath = Path.Combine(_env.WebRootPath, newKapakUrl);

            using (var stream = new FileStream(newPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var oldPath = Path.Combine(_env.WebRootPath, urun.KapakUrl);

            if (File.Exists(oldPath))
                File.Delete(oldPath);

            urun.KapakUrl = newKapakUrl;
        }

        return new();
    }

    public async Task<List<Urun>> GetYeniEklenenlerAsync(int count)
        => await GetQueryable(queryable: _set.OrderByDescending(u => u.EklenmeTarihi),
                              include: u => u.Kategori,
                              tracked: false,
                              page: 1,
                              pageSize: count)
            .ToListAsync();

    public async Task<List<Urun>> GetIndirimdekilerAsync(int count)
        => await GetQueryable(queryable: _set.OrderByDescending(u => u.IndirimYuzdesi),
                              filter: u => u.IndirimYuzdesi > 0,
                              include: u => u.Kategori,
                              tracked: false,
                              page: 1,
                              pageSize: count)
        .ToListAsync();

    public async Task<List<Urun>> GetOfKategori(int kategoriId, int count)
        => await _set
        .AsNoTracking()
        .Where(u => u.KategoriId == kategoriId)
        .OrderByDescending(u => u.EklenmeTarihi)
        .Take(count)
        .ToListAsync();
}
