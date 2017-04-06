using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Silk.Signals;
using Silk.Web.Core.Abstractions;
using System.Linq;
using System.Linq.Expressions;

namespace Silk.Web.Core
{
	public class InMemoryAccountRepository<TAccount> : IAccountRepository<TAccount> where TAccount : IAccount
	{
		private readonly List<TAccount> _accounts = new List<TAccount>();
		private readonly Dictionary<TAccount, IAccountCredentials> _accountCredentials = new Dictionary<TAccount, IAccountCredentials>();

		public event SignalHandler<TAccount> AccountCreated;
		public event SignalHandler<TAccount> AccountCredentialsUpdated;

		public void CreateAccount(TAccount account)
		{
			_accounts.Add(account);
			AccountCreated?.Invoke(account);
		}

		public Task CreateAccountAsync(TAccount account)
		{
			CreateAccount(account);
			return Task.FromResult(true);
		}

		public TAccount GetAccountByCredentials(IAccountCredentials credentials)
		{
			foreach(var kvp in _accountCredentials)
			{
				if (kvp.Value.CompareCredentials(credentials))
					return kvp.Key;
			}
			return default(TAccount);
		}

		public Task<TAccount> GetAccountByCredentialsAsync(IAccountCredentials credentials)
		{
			return Task.FromResult(GetAccountByCredentials(credentials));
		}

		public TAccount GetAccountById(Guid accountId)
		{
			return _accounts.FirstOrDefault(q => q.AccountId.Equals(accountId));
		}

		public Task<TAccount> GetAccountByIdAsync(Guid accountId)
		{
			return Task.FromResult(GetAccountById(accountId));
		}

		public TAccount[] GetAccountsById(IEnumerable<Guid> accountIds)
		{
			return _accounts.Where(q => accountIds.Contains(q.AccountId)).ToArray();
		}

		public Task<TAccount[]> GetAccountsByIdAsync(IEnumerable<Guid> accountIds)
		{
			return Task.FromResult(GetAccountsById(accountIds));
		}

		public void SetAccountCredentials(TAccount account, IAccountCredentials credentials)
		{
			_accountCredentials[account] = credentials;
			AccountCredentialsUpdated?.Invoke(account);
		}

		public Task SetAccountCredentialsAsync(TAccount account, IAccountCredentials credentials)
		{
			SetAccountCredentials(account, credentials);
			return Task.FromResult(true);
		}

		public TProjection[] Find<TProjection>(AccountCriteria<TAccount> criteria, Expression<Func<TAccount, TProjection>> projectionFunc = null)
		{
			var enumerable = BuildLinqQuery(criteria, projectionFunc);
			return enumerable.ToArray();
		}

		public Task<TProjection[]> FindAsync<TProjection>(AccountCriteria<TAccount> criteria, Expression<Func<TAccount, TProjection>> projectionFunc = null)
		{
			return Task.FromResult(Find(criteria, projectionFunc));
		}

		private IEnumerable<TProjection> BuildLinqQuery<TProjection>(AccountCriteria<TAccount> criteria, Expression<Func<TAccount, TProjection>> projectionFunc)
		{
			IEnumerable<TAccount> query = _accounts;

			query = query.Skip(
				(criteria.RequestPage - 1) * criteria.RequestRecordsPerPage
				).Take(
				criteria.RequestRecordsPerPage
				);

			if (!string.IsNullOrEmpty(criteria.DisplayNameContainsQuery))
				query = query.Where(q => q.DisplayName.IndexOf(criteria.DisplayNameContainsQuery, StringComparison.OrdinalIgnoreCase) > -1);

			if (projectionFunc != null)
			{
				return query.Select(projectionFunc.Compile());
			}
			else
			{
				return query.OfType<TProjection>();
			}
		}
	}
}
