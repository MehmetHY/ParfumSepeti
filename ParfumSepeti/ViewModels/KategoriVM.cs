namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KategoriVM : PagedVM<UrunCardVM>
{
    public int Id { get; set; }
    public string Isim { get; set; }
}
