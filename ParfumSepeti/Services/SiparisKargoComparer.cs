using ParfumSepeti.Const;

namespace ParfumSepeti.Services;

public class SiparisKargoComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y)
            return 0;

        if (x == KargoDurumu.GONDERILMEDI)
            return -1;

        return 1;
    }
}
