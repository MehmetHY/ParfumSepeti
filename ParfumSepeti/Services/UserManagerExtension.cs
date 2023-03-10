using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Const;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Services;

public static class UserManagerExtension
{
    public static async Task<Result<Kullanici>> CreateAsync(
        this UserManager<Kullanici> userManager,
        KaydolVM model
    )
    {
        var kullanici = new Kullanici
        {
            UserName = model.KullaniciAdi
        };

        var identityResult = await userManager.CreateAsync(kullanici, model.Sifre);
        var result = new Result<Kullanici>(identityResult);
        result.Object = result.Success ? kullanici : null;

        return result;
    }
}
