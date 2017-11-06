using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silk.Web.Core.Persistence
{
	public interface IAccountRolesRepository
	{
		Task<List<string>> GetAccountRolesAsync(IAccountIdentifier account);
		Task SetAccountRolesAsync(IAccountIdentifier account, IEnumerable<string> roles);
	}
}
