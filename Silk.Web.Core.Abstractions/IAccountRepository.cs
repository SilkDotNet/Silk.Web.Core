using Silk.Signals;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface IAccountRepository<TAccount> where TAccount : IAccount
	{
		event SignalHandler<TAccount> AccountCreated;
		event SignalHandler<TAccount> AccountCredentialsUpdated; 

		TAccount GetAccountById(Guid accountId);
		Task<TAccount> GetAccountByIdAsync(Guid accountId);
		TAccount[] GetAccountsById(IEnumerable<Guid> accountIds);
		Task<TAccount[]> GetAccountsByIdAsync(IEnumerable<Guid> accountIds);

		TAccount GetAccountByCredentials(IAccountCredentials credentials);
		Task<TAccount> GetAccountByCredentialsAsync(IAccountCredentials credentials);

		void CreateAccount(TAccount account);
		Task CreateAccountAsync(TAccount account);
		void SetAccountCredentials(TAccount account, IAccountCredentials credentials);
		Task SetAccountCredentialsAsync(TAccount account, IAccountCredentials credentials);

		TProjection[] Find<TProjection>(AccountCriteria<TAccount> criteria, Expression<Func<TAccount, TProjection>> projectionFunc = null);
		Task<TProjection[]> FindAsync<TProjection>(AccountCriteria<TAccount> criteria, Expression<Func<TAccount, TProjection>> projectionFunc = null);
	}

	public class AccountCriteria<TAccount> where TAccount : IAccount
	{
		public int RequestPage { get; private set; } = 1;
		public int RequestRecordsPerPage { get; private set; } = 20;
		public string DisplayNameContainsQuery { get; private set; }
		public string DisplayNameExactQuery { get; private set; }

		public AccountCriteria<TAccount> DisplayNameContains(string searchQuery)
		{
			DisplayNameContainsQuery = searchQuery;
			return this;
		}

		public AccountCriteria<TAccount> DisplayNameExact(string searchQuery)
		{
			DisplayNameExactQuery = searchQuery;
			return this;
		}

		public AccountCriteria<TAccount> Page(int page)
		{
			RequestPage = page;
			return this;
		}

		public AccountCriteria<TAccount> RecordsPerPage(int recordsPerPage)
		{
			RequestRecordsPerPage = recordsPerPage;
			return this;
		}
	}
}
