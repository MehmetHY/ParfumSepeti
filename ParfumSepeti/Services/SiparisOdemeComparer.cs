using ParfumSepeti.Const;

namespace ParfumSepeti.Services;

public class SiparisOdemeComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y)
            return 0;

        if (x == OdemeDurumu.ODENDI)
            return -1;

        return 1;
    }
}
