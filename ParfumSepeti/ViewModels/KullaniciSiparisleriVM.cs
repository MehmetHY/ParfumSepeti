namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KullaniciSiparisleriVM
{
    public class SiparisItem
    {
        public class Oge
        {
            public string UrunIsmi { get; set; }

            public decimal Fiyat { get; set; }

            public int Adet { get; set; }
        }

        public int Id { get; set; }

        public List<Oge> Ogeler { get; set; } = new();

        public decimal Toplam { get; set; }

        public DateTime OlusuturmaTarihi { get; set; }

        public string OdemeDurumu { get; set; }

        public string KargoDurumu { get; set; }
    }

    public List<SiparisItem> Items { get; set; } = new();
}
