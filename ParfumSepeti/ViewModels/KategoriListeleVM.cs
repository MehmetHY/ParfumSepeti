namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KategoriListeleItem
{
    public int Id { get; set; }

    public string Isim { get; set; }

    public int UrunSayisi { get; set; }
}

public class KategoriListeleVM : PagedVM<KategoriListeleItem>
{
}
