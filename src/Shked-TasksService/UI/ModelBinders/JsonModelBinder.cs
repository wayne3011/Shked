using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ShkedTasksService.UI.ModelBinders;

public class JsonModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult != ValueProviderResult.None)
        {
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var valueString = valueProviderResult.FirstValue;
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject(valueString, bindingContext.ModelType);
            if (result != null)
            {
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

        }
        return Task.CompletedTask;
    }
}