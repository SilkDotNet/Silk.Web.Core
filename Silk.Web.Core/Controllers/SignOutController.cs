using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Silk.Web.Core.Controllers
{
	[Route("~/{route:featureRoute(" + CoreFeatures.SignOutFeature + ")}")]
	public class SignOutController : Controller
	{
		public virtual async Task<IActionResult> Index([FromQuery] string returnUrl = null)
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Redirect(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/");
		}
	}
}
