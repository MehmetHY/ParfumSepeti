using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Primitives;

namespace ParfumSepeti.ViewModels;

public abstract class PagedVM<TItem> : PagedVM where TItem : class
{
    [ValidateNever]
    public List<TItem> Items { get; set; } = new();
}

public abstract class PagedVM
{
    [ValidateNever]
    public int CurrentPage { get; set; } = 1;

    [ValidateNever]
    public int PageSize { get; set; } = 20;

    [ValidateNever]
    public int LastPage { get; set; }

    public string GetQueryString(
        string path,
        IEnumerable<KeyValuePair<string, StringValues>> query,
        int page,
        int pageSize
    )
    {
        var list = query.ToList();
        list.Add(new("page", page.ToString()));
        list.Add(new("pageSize", pageSize.ToString()));
        var queryString = QueryString.Create(list);

        return $"{path}{queryString}";
    }
}
