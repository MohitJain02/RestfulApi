using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public class ArrayModelBinders : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // our binder works only on enumerable types
            if(!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // Gets the input value from the value provider

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            // If the value is null or whitespace then return null
            if(string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // value is isn't null or whitespace
            // and the type of the model is enumerable
            // Get the enuermable type and converter
            var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            var converter = TypeDescriptor.GetConverter(elementType);

            // convert each item in the value list to the enumerable type
            var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => converter.ConvertFromString(x.Trim()))
                                    .ToArray();

            // create an array of that type and set it as the model value
            var typedValue = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValue, 0);
            bindingContext.Model = typedValue;

            // return a successful result passing in the model
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
