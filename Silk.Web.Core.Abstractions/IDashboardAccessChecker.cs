using System.Security.Claims;
using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface IDashboardAccessChecker
	{
		bool CanAccess(ClaimsPrincipal user);
		Task<bool> CanAccessAsync(ClaimsPrincipal user);
	}
}
