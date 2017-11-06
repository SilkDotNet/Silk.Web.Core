using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Silk.Web.Core.Validation
{
	/// <summary>
	/// Populates a member with the currently claimed user.
	/// </summary>
	public class ClaimedUserAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			//  todo: don't use reflection always here, cache it
			var property = validationContext.ObjectInstance.GetType()
				.GetProperty(validationContext.DisplayName);
			if (property != null)
			{
				var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
				property.SetValue(
					validationContext.ObjectInstance, httpContextAccessor.HttpContext.User.GetClaimedAccount()
					);
			}

			return ValidationResult.Success;
		}
	}
}
