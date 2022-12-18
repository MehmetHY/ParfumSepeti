namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class SepetItem
{
    public int Id { get; set; }

    public string KapakUrl { get; set; }

    public string Baslik { get; set; }

    public string Model { get; set; }

    public int Adet { get; set; }

    public decimal BirimFiyat { get; set; }
}

public class SepetVM
{
    public List<SepetItem> Items { get; set; } = new();

    public string Toplam { get; set; }
}


public class SepetSessionObject
{
    public class SepetItemSessionObject
    {
        public int UrunId { get; set; } = 0;

        public int Adet { get; set; } = 1;
    }

    public List<SepetItemSessionObject> Items { get; set; } = new();
}