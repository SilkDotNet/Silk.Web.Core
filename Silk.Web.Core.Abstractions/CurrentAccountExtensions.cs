using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Silk.Web.Core
{
	public static class CurrentAccountExtensions
	{
		public static Account GetClaimedAccount(this ClaimsPrincipal claimsPrincipal)
		{
			var claimedId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
			var claimedDisplayName = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

			if (claimedId != null && claimedDisplayName != null)
				return new ClaimedAccount(Guid.Parse(claimedId.Value), claimedDisplayName.Value);
			return null;
		}

		public static Account GetClaimedAccount(this Controller controller)
		{
			return controller.User.GetClaimedAccount();
		}
	}
}
