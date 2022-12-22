using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class AdminSiparisDuzenleVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Kargo Durumu")]
    public string KargoDurumu { get; set; }

    [ValidateNever]
    public List<SelectListItem> KargoDurumlari { get; set; }

    [Display(Name = "Taşıyıcı Firma")]
    public string? TasiyiciFirma { get; set; }

    [Display(Name = "Kargo Takip Numarası")]
    public string? KargoTakipNo { get; set; }
}
