using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;
using System;

namespace Silk.Web.Core.Binding
{
	public class IdentifierBinding<T> : IModelBinder
		where T : new()
	{
		private readonly PropertyInfo _idProperty;

		public IdentifierBinding()
		{
			_idProperty = typeof(T).GetProperty("Id");
		}

		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			var modelName = bindingContext.BinderModelName;
			if (modelName == null)
				modelName = bindingContext.ModelName;

			var valueProviderResult =
				bindingContext.ValueProvider.GetValue(modelName);

			if (valueProviderResult == ValueProviderResult.None)
			{
				return Task.CompletedTask;
			}

			bindingContext.ModelState.SetModelValue(modelName,
				valueProviderResult);

			var value = valueProviderResult.FirstValue;

			// Check if the argument value is null or empty
			if (string.IsNullOrEmpty(value))
			{
				return Task.CompletedTask;
			}

			Guid id = Guid.Empty;
			if (!Guid.TryParse(value, out id))
			{
				// Non-integer arguments result in model state errors
				bindingContext.ModelState.TryAddModelError(
										bindingContext.ModelName,
										"Id must be a guid.");
				return Task.CompletedTask;
			}

			var model = new T();
			_idProperty.SetValue(model, id);

			bindingContext.Result = ModelBindingResult.Success(model);
			return Task.CompletedTask;
		}
	}
}
