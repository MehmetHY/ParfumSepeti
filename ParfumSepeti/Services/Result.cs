using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ParfumSepeti.Services;

public class Result
{
    public Result()
    {
        Success = true;
        Errors = new();
    }

    public Result(IdentityResult identityResult)
    {
        Success = identityResult.Succeeded;
        Errors = new();

        foreach (var err in identityResult.Errors)
            Errors.Add(err.Description);
    }

    public bool Success { get; set; }

    public List<string> Errors { get; set; }

    public override string? ToString() => string.Join(", ", Errors);
}

public class Result<T> : Result
{
    public Result() : base()
    {
    }

    public Result(IdentityResult identityResult) : base(identityResult)
    {
    }

    public T? Object { get; set; }
}
