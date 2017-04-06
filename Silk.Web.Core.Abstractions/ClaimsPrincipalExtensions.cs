using System;
using System.Linq;
using System.Security.Claims;

namespace Silk.Web.Core.Abstractions
{
	public static class ClaimsPrincipalExtensions
	{
		public static Guid? GetAccountId(this ClaimsPrincipal claimsPrincipal)
		{
			var accountIdClaim = claimsPrincipal.Claims.FirstOrDefault(q => q.Type == ClaimTypes.Sid);
			if (accountIdClaim == null)
				return null;
			return Guid.Parse(accountIdClaim.Value);
		}
	}
}
