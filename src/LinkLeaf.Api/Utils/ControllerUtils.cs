using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LinkLeaf.Api.Utils;

public static class ControllerUtils
{
    public static Dictionary<string, List<string>> GetModelErrors(ModelStateDictionary modelState)
    {
        Dictionary<string, List<string>> errors = [];
        foreach (var key in modelState.Keys)
        {
            var modelStateEntry = modelState[key];
            if (modelStateEntry?.Errors.Count > 0)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    if (!errors.ContainsKey(key))
                        errors[key] = [];
                    errors[key].Add(error.ErrorMessage);
                }
            }
        }
        return errors;
    }
}
