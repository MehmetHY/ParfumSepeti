using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ParfumSepeti.Services;

public static class ModelStateExtension
{
    public static void AddResultErrors(this ModelStateDictionary model, Result result)
    {
        foreach (var error in result.Errors)
            model.AddModelError(string.Empty, error);
    }
}
