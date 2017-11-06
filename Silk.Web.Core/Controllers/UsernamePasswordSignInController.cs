using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Silk.Web.Core.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Silk.Web.Core.Controllers
{
	[Route("~/{route:featureRoute(" + CoreFeatures.UsernamePasswordSignInFeature + ")}")]
	public class UsernamePasswordSignInController : Controller
	{
		[HttpGet]
		public IActionResult Index([FromQuery] string returnUrl = null)
		{
			return View(new SignInForm { ReturnUrl = returnUrl });
		}

		[HttpPost]
		public async Task<IActionResult> Index([FromForm] SignInForm model,
			[FromServices] ICredentialsManager<UsernamePasswordCredentials, UserAccount> credentialsManager,
			[FromServices] IAccountRolesRepository accountRoles)
		{
			if (ModelState.IsValid)
			{
				var credentials = new UsernamePasswordCredentials(model.UserId, model.Password);
				var account = await credentialsManager.GetAccountByCredentialsAsync(credentials)
					.ConfigureAwait(false);
				if (account == null)
				{
					ModelState.AddModelError("", "Username or password is invalid.");
				}
				else
				{
					await SetPrincipalAsync(account, model.RememberMe, await accountRoles.GetAccountRolesAsync(account)
						.ConfigureAwait(false))
						.ConfigureAwait(false);
					return Redirect(!string.IsNullOrEmpty(model.ReturnUrl) ? model.ReturnUrl : "~/");
				}
			}
			return View(model);
		}

		protected virtual Task SetPrincipalAsync(UserAccount account, bool rememberMe, IEnumerable<string> roles)
		{
			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, account.DisplayName),
				new Claim(ClaimTypes.Sid, account.Id.ToString()),
				new Claim("loginTime", DateTime.UtcNow.ToString())
			};
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			var loginPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "SilkCoreIdentity"));
			return HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				loginPrincipal,
				new AuthenticationProperties
				{
					IsPersistent = rememberMe
				});
		}

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
}
