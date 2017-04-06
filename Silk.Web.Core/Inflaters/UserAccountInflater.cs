using Silk.Mapping;
using Silk.Web.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Web.Core.Inflaters
{
	[SupportedSuffix("Id")]
	[SupportedSuffix("AccountId")]
	public class UserAccountInflater : IInflater<Guid, UserAccount>
	{
		private readonly IAccountRepository<UserAccount> _userAccountRepository;

		public UserAccountInflater(IAccountRepository<UserAccount> userAccountRepository)
		{
			_userAccountRepository = userAccountRepository;
		}

		public IEnumerable<UserAccount> Inflate(IEnumerable<Guid> source)
		{
			var accounts = _userAccountRepository.GetAccountsById(source);
			foreach (var id in source)
				yield return accounts.FirstOrDefault(q => q.AccountId.Equals(id));
		}

		public async Task<IEnumerable<UserAccount>> InflateAsync(IEnumerable<Guid> source)
		{
			var accounts = await _userAccountRepository.GetAccountsByIdAsync(source).ConfigureAwait(false);
			var ret = new List<UserAccount>();
			foreach (var id in source)
				ret.Add(accounts.FirstOrDefault(q => q.AccountId.Equals(id)));
			return ret;
		}
	}
}
