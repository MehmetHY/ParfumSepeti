namespace ParfumSepeti.ViewModels;

public class KullaniciListeleItem
{
    public string Id { get; set; }

    public string Isim { get; set; }

    public int OdenmisSiparis { get; set; }
}

public class KullaniciListeleVM : PagedVM<KullaniciListeleItem>
{
}
