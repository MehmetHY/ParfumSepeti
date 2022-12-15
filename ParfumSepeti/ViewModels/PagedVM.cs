using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ParfumSepeti.ViewModels;

public abstract class PagedVM<TItem> where TItem : class
{
    [ValidateNever]
    public List<TItem> Items { get; set; } = new();

    [ValidateNever]
    public int CurrentPage { get; set; } = 1;

    [ValidateNever]
    public int PageSize { get; set; }

    public int GetTotalPageCount()
    {
        var total = Items.Count / PageSize;

        if (Items.Count % PageSize != 0)
            ++total;

        return total;
    }
}
