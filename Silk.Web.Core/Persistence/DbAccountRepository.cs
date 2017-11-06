using System;
using System.Threading.Tasks;
using System.Linq;
using Silk.Data.SQL.Providers;
using Silk.Signals;
using Silk.Data.SQL.Expressions;
using System.Collections.Generic;

namespace Silk.Web.Core.Persistence
{
	public class DbAccountRepository<TAccount> : DbRepositoryBase<TAccount>,
		IAccountRepository<TAccount>
		where TAccount : Account, new()
	{
		public AsyncSignal<TAccount> AccountCreated { get; } = new AsyncSignal<TAccount>();
		public AsyncSignal<TAccount> AccountUpdated { get; } = new AsyncSignal<TAccount>();
		public AsyncSignal<TAccount> AccountDeleted { get; } = new AsyncSignal<TAccount>();

		public DbAccountRepository(IDataProvider database)
			: base(database)
		{
		}

		public Task CreateAccountsAsync(params TAccount[] accounts)
		{
			if (accounts.Any(account => !account.Id.Equals(Guid.Empty)))
				throw new ArgumentException("Account ids must be empty.", nameof(accounts));

			return base.CreateAsync(accounts, AccountCreated);
		}

		public Task UpdateAccountsAsync(params TAccount[] accounts)
		{
			if (accounts.Any(account => account.Id.Equals(Guid.Empty)))
				throw new ArgumentException("Account ids must not be empty.", nameof(accounts));

			return base.UpdateAsync(accounts, AccountUpdated);
		}

		public Task DeleteAccountsAsync(params TAccount[] accounts)
		{
			if (accounts.Any(account => account.Id.Equals(Guid.Empty)))
				throw new ArgumentException("Account ids must not be empty.", nameof(accounts));

			return base.DeleteAsync(accounts, AccountDeleted);
		}

		public async Task<TAccount> GetAccountByIdAsync(Guid id)
		{
			return (await SelectAsync(
				where: QueryExpression.Compare(QueryExpression.Column(nameof(Account.Id)), ComparisonOperator.AreEqual, QueryExpression.Value(id)),
				limit: 1)
				.ConfigureAwait(false))
				.FirstOrDefault();
		}

		public async Task<TView> GetAccountByIdAsync<TView>(Guid id) where TView : new()
		{
			return (await SelectAsync<TView>(
				where: QueryExpression.Compare(QueryExpression.Column(nameof(Account.Id)), ComparisonOperator.AreEqual, QueryExpression.Value(id)),
				limit: 1)
				.ConfigureAwait(false))
				.FirstOrDefault();
		}

		public Task<ICollection<TAccount>> FindAccountsAsync(AccountCriteria criteria)
		{
			return SelectAsync(
				where: CriteriaToCondition(criteria)
				);
		}

		public Task<ICollection<TView>> FindAccountsAsync<TView>(AccountCriteria criteria)
			where TView : new()
		{
			return SelectAsync<TView>(
				where: CriteriaToCondition(criteria)
				);
		}

		private QueryExpression CriteriaToCondition(AccountCriteria criteria)
		{
			QueryExpression where = null;

			if (criteria.MatchAccountIdentifier != null)
				where = CombineConditions(where, ConditionType.AndAlso, QueryExpression.Compare(
					QueryExpression.Column(nameof(Account.Id)), ComparisonOperator.AreEqual, QueryExpression.Value(criteria.MatchAccountIdentifier.Id)
					));

			if (criteria.MatchDisplayName != null)
				where = CombineConditions(where, ConditionType.AndAlso, QueryExpression.Compare(
					QueryExpression.Column(nameof(Account.DisplayName)), ComparisonOperator.AreEqual, QueryExpression.Value(criteria.MatchDisplayName)
					));

			return where;
		}

		private QueryExpression CombineConditions(QueryExpression existing, ConditionType conditionType, QueryExpression @new)
		{
			if (existing == null)
				return @new;
			if (conditionType == ConditionType.AndAlso)
				return QueryExpression.AndAlso(existing, @new);
			return QueryExpression.OrElse(existing, @new);
		}
	}
}
