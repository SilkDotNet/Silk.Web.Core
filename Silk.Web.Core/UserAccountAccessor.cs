using System.Threading.Tasks;
using Silk.Web.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Silk.Web.Core
{
	internal class UserAccountAccessor : IAccountAccessor<UserAccount>
	{
		private readonly IAccountRepository<UserAccount> _accountRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private UserAccount _cachedAccount;

		public UserAccountAccessor(IAccountRepository<UserAccount> accountRepository,
			IHttpContextAccessor httpContextAccessor)
		{
			_accountRepository = accountRepository;
			_httpContextAccessor = httpContextAccessor;
		}

		public UserAccount GetCurrentAccount()
		{
			if (_cachedAccount != null)
				return _cachedAccount;

			var accountId = _httpContextAccessor.HttpContext.User.GetAccountId();
			if (accountId == null || !accountId.HasValue)
				return null;
			_cachedAccount = _accountRepository.GetAccountById(accountId.Value);

			return _cachedAccount;
		}

		public async Task<UserAccount> GetCurrentAccountAsync()
		{
			if (_cachedAccount != null)
				return _cachedAccount;

			var accountId = _httpContextAccessor.HttpContext.User.GetAccountId();
			if (accountId == null || !accountId.HasValue)
				return null;
			_cachedAccount = await _accountRepository.GetAccountByIdAsync(accountId.Value).ConfigureAwait(false);

			return _cachedAccount;
		}
	}
}
