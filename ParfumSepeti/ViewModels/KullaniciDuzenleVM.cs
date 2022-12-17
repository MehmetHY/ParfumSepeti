using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KullaniciDuzenleVM
{
    [ValidateNever]
    public string Id { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [RegularExpression("""^[a-zA-Z0-9-_\.]{3,50}$""",
        ErrorMessage = "En az 3, en fazla 50 karakter olmalı. '-', '_' ve '.' dışında özel karakter bulunduramaz!")]
    [Display(Name = "Kullanıcı Adı")]
    public string KullaniciAdi { get; set; }
}
