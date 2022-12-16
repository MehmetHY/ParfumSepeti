using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KategoriDuzenleVM
{
    [ValidateNever]
    public int Id { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    public string Isim { get; set; }
}
