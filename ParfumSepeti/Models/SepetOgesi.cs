namespace ParfumSepeti.Models;

#pragma warning disable CS8618

public class SepetOgesi
{
    public int Id { get; set; }

    public Urun Urun { get; set; }

    public int Adet { get; set; } = 1;

    public Sepet Sepet { get; set; }
}
