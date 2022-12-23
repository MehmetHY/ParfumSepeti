namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class AdminSiparisListeleVM : PagedVM<AdminSiparisListeleVM.Siparis>
{
    public class Siparis
    {
        public class Oge
        {
            public string UrunIsmi { get; set; }

            public decimal Fiyat { get; set; }

            public int Adet { get; set; }
        }

        public int Id { get; set; }

        public string Kullanici { get; set; }

        public List<Oge> Ogeler { get; set; } = new();

        public string Toplam { get; set; }

        public DateTime OlusuturmaTarihi { get; set; }

        public string OdemeDurumu { get; set; }

        public string KargoDurumu { get; set; }
    }
}
