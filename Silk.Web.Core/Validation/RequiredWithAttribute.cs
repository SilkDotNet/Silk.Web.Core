using System.ComponentModel.DataAnnotations;

namespace Silk.Web.Core.Validation
{
	public class RequiredWithAttribute : ValidationAttribute
	{
		public RequiredWithAttribute(string withPropertyName)
		{
			WithPropertyName = withPropertyName;
		}

		public string WithPropertyName { get; }

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var property = validationContext.ObjectInstance.GetType()
				.GetProperty(WithPropertyName);
			var propertyValue = property.GetValue(validationContext.ObjectInstance);

			if (property.PropertyType == typeof(bool) &&
				(bool)propertyValue == true)
			{
				if (value == null)
				{
					return new ValidationResult($"{validationContext.DisplayName} is required with {WithPropertyName}.");
				}
			}

			return ValidationResult.Success;
		}
	}
}
