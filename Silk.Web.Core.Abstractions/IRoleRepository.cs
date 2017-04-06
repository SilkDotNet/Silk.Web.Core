using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface IRoleRepository<TAccount> where TAccount : IAccount
	{
		string[] GetRoles(TAccount account);
		Task<string[]> GetRolesAsync(TAccount account);
		void GrantRoles(TAccount account, params string[] roles);
		Task GrantRolesAsync(TAccount account, params string[] roles);
		void RevokeRoles(TAccount account, params string[] roles);
		Task RevokeRolesAsync(TAccount account, params string[] roles);
	}
}
