using Microsoft.AspNetCore.Identity;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Services;

public static class SignInManager
{
    public static async Task<Result> SignInAsync(this SignInManager<Kullanici> signInManager,
                                          UserManager<Kullanici> userManager,
                                          GirisVM model)
    {
        var kullanici = await userManager.FindByNameAsync(model.KullaniciAdi);

        if (kullanici == null)
            return new()
            {
                Success = false,
                Errors = { "Hatalı kullanıcı adı veya şifre" }
            };

        var passwordCorrect = await userManager.CheckPasswordAsync(kullanici,
                                                                   model.Sifre);

        if (!passwordCorrect)
            return new()
            {
                Success = false,
                Errors = { "Hatalı kullanıcı adı veya şifre" }
            };

        await signInManager.SignInAsync(kullanici, model.BeniHatirla);

        return new();
    }
}
