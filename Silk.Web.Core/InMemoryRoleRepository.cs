using System;
using System.Threading.Tasks;
using Silk.Web.Core.Abstractions;
using System.Collections.Generic;

namespace Silk.Web.Core
{
	public class InMemoryRoleRepository<TAccount> : IRoleRepository<TAccount> where TAccount : IAccount
	{
		private Dictionary<Guid, List<string>> _roles = new Dictionary<Guid, List<string>>();

		private List<string> GetRolesList(TAccount account)
		{
			var accountId = account.AccountId;
			List<string> roles;
			lock (_roles)
			{
				if (_roles.TryGetValue(accountId, out roles))
					return roles;
				roles = new List<string>();
				_roles[accountId] = roles;
			}
			return roles;
		}

		public string[] GetRoles(TAccount account)
		{
			var rolesList = GetRolesList(account);
			lock (rolesList)
				return rolesList.ToArray();
		}

		public Task<string[]> GetRolesAsync(TAccount account)
		{
			return Task.FromResult(GetRoles(account));
		}

		public void GrantRoles(TAccount account, params string[] roles)
		{
			var accountRoles = GetRolesList(account);
			lock (accountRoles)
			{
				foreach (var role in roles)
					if (!accountRoles.Contains(role))
						accountRoles.Add(role);
			}
		}

		public Task GrantRolesAsync(TAccount account, params string[] roles)
		{
			GrantRoles(account, roles);
			return Task.FromResult(true);
		}

		public void RevokeRoles(TAccount account, params string[] roles)
		{
			var accountRoles = GetRolesList(account);
			lock (accountRoles)
			{
				foreach (var role in roles)
					accountRoles.Remove(role);
			}
		}

		public Task RevokeRolesAsync(TAccount account, params string[] roles)
		{
			RevokeRoles(account, roles);
			return Task.FromResult(true);
		}
	}
}
