using Silk.Web.Core.Persistence;
using System.Threading.Tasks;

namespace Silk.Web.Core.Services
{
	public class UserManager
	{
		private readonly IAccountRepository<UserAccount> _users;
		private readonly ICredentialsManager<UsernamePasswordCredentials, UserAccount> _credentialsManager;
		private readonly IAccountRolesRepository _roles;

		public UserManager(IAccountRepository<UserAccount> users,
			ICredentialsManager<UsernamePasswordCredentials, UserAccount> credentialsManager,
			IAccountRolesRepository roles)
		{
			_users = users;
			_credentialsManager = credentialsManager;
			_roles = roles;
		}

		public async Task CreateUserAsync(UserAccount user, string[] roles = null, string password = null)
		{
			await _users.CreateAccountsAsync(user)
				.ConfigureAwait(false);
			if (roles != null)
			{
				await _roles.SetAccountRolesAsync(user, roles)
					.ConfigureAwait(false);
			}
			if (!string.IsNullOrEmpty(password))
			{
				await _credentialsManager.SetAccountCredentialsAsync(user,
					new UsernamePasswordCredentials(null, password));
			}
		}
	}
}
