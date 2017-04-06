using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Silk.Web.Core.ViewModels
{
	public class SignInForm
	{
		[Display(Name = "Login", Prompt = "Username or e-mail address")]
		[Required]
		public string UserId { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		[Required]
		public string Password { get; set; }

		public bool RememberMe { get; set; }

		[HiddenInput]
		public string ReturnUrl { get; set; }
	}
}
