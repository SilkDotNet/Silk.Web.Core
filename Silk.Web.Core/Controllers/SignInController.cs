using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Silk.Web.Core.Abstractions;
using Silk.Web.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Silk.Web.Core.Controllers
{
	public class SignInController : Controller
	{
		private readonly IAccountRepository<UserAccount> _accountRepository;
		private readonly IRoleRepository<UserAccount> _roleRepository;

		public SignInController(IAccountRepository<UserAccount> accountRepository,
			IRoleRepository<UserAccount> roleRepository)
		{
			_accountRepository = accountRepository;
			_roleRepository = roleRepository;
		}

		[HttpGet]
		public virtual IActionResult Index([FromQuery] string returnUrl = null)
		{
			var model = new SignInForm { ReturnUrl = returnUrl };
			return View(model);
		}

		[HttpPost]
		public virtual async Task<IActionResult> Index([FromForm] SignInForm model)
		{
			if (ModelState.IsValid)
			{
				var userAccount = await _accountRepository.GetAccountByCredentialsAsync(new UsernamePasswordCredentials(model.UserId, model.Password)).ConfigureAwait(false);
				if (userAccount == null)
				{
					ModelState.AddModelError("", "Username or password is invalid.");
				}
				else
				{
					await SetPrincipalAsync(userAccount, model.RememberMe,
						await _roleRepository.GetRolesAsync(userAccount).ConfigureAwait(false)).ConfigureAwait(false);
					return Redirect(model.ReturnUrl ?? "~/");
				}
			}
			return View(model);
		}

		public virtual async Task<IActionResult> SignOut([FromQuery] string returnUrl = null)
		{
			await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Redirect(returnUrl ?? "~/");
		}

		protected async Task SetPrincipalAsync(UserAccount account, bool rememberMe, string[] roles)
		{
			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, account.DisplayName),
				new Claim(ClaimTypes.Sid, account.AccountId.ToString()),
				new Claim(ClaimTypes.NameIdentifier, account.Slug),
				new Claim("loginTime", DateTime.UtcNow.ToString())
			};
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			var loginPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "SilkCoreIdentity"));
			await HttpContext.Authentication.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				loginPrincipal,
				new AuthenticationProperties
				{
					IsPersistent = rememberMe
				});
		}
	}
}
