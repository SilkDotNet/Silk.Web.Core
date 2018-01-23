using System;
using System.Threading.Tasks;
using System.Linq;
using Silk.Data.SQL.Expressions;
using System.Collections.Generic;
using Silk.Web.Core.Data;

namespace Silk.Web.Core.Persistence
{
	public class DbAccountRepository<TAccount> : DbRepositoryBase<TAccount>,
		IAccountRepository<TAccount>
		where TAccount : Account, new()
	{
		public DbAccountRepository(IDatabase<TAccount> database)
			: base(database)
		{
		}

		public Task CreateAccountsAsync(params TAccount[] accounts)
		{
			if (accounts.Any(account => !account.Id.Equals(Guid.Empty)))
				throw new ArgumentException("Account ids must be empty.", nameof(accounts));

			return base.CreateAsync(accounts);
		}

		public Task UpdateAccountsAsync(params TAccount[] accounts)
		{
			if (accounts.Any(account => account.Id.Equals(Guid.Empty)))
				throw new ArgumentException("Account ids must not be empty.", nameof(accounts));

			return base.UpdateAsync(accounts);
		}

		public Task DeleteAccountsAsync(params TAccount[] accounts)
		{
			if (accounts.Any(account => account.Id.Equals(Guid.Empty)))
				throw new ArgumentException("Account ids must not be empty.", nameof(accounts));

			return base.DeleteAsync(accounts);
		}

		public Task<ICollection<TAccount>> GetAccountsByIdAsync(params Guid[] ids)
		{
			return SelectAsync(
				where: QueryExpression.Compare(QueryExpression.Column(nameof(Account.Id)), ComparisonOperator.None, QueryExpression.InFunction(ids.OfType<object>().ToArray()))
				);
		}

		public Task<ICollection<TView>> GetAccountsByIdAsync<TView>(params Guid[] ids) where TView : new()
		{
			return SelectAsync<TView>(
				where: QueryExpression.Compare(QueryExpression.Column(nameof(Account.Id)), ComparisonOperator.None, QueryExpression.InFunction(ids.OfType<object>().ToArray()))
				);
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
