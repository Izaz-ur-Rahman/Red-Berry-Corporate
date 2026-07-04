using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RedBerryCorporate.Helpers
{
    public static class ValidationHelper
    {
        public static Dictionary<string, List<string>> GetErrors(ModelStateDictionary modelState)
        {
            return modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => char.ToLowerInvariant(x.Key[0]) + x.Key.Substring(1),
                    x => x.Value!.Errors
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Invalid value."
                            : e.ErrorMessage)
                        .ToList()
                );
        }
    }
}