using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KategoriSilVM
{
    [ValidateNever]
    public string Isim { get; set; }

    [ValidateNever]
    public int UrunSayisi { get; set; }
}
