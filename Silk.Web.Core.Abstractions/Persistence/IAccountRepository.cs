using Silk.Signals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silk.Web.Core.Persistence
{
	/// <summary>
	/// Stores and retrieves accounts of a given type.
	/// </summary>
	/// <typeparam name="TAccount"></typeparam>
	public interface IAccountRepository<TAccount> where TAccount : Account, new()
	{
		AsyncSignal<TAccount> AccountCreated { get; }
		AsyncSignal<TAccount> AccountUpdated { get; }
		AsyncSignal<TAccount> AccountDeleted { get; }

		/// <summary>
		/// Creates a new persistent store for an account.
		/// </summary>
		/// <param name="adHost"></param>
		/// <returns></returns>
		Task CreateAccountsAsync(params TAccount[] accounts);

		/// <summary>
		/// Updates the persistent store for an account.
		/// </summary>
		/// <param name="adHost"></param>
		/// <returns></returns>
		Task UpdateAccountsAsync(params TAccount[] accounts);

		/// <summary>
		/// Deletes the persisten store for an account.
		/// </summary>
		/// <param name="adHost"></param>
		/// <returns></returns>
		Task DeleteAccountsAsync(params TAccount[] accounts);

		/// <summary>
		/// Gets a single account by id from the persistent store.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<ICollection<TAccount>> GetAccountsByIdAsync(params Guid[] ids);

		/// <summary>
		/// Gets a view of a single account by id from the persistent store.
		/// </summary>
		/// <typeparam name="TView"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<ICollection<TView>> GetAccountsByIdAsync<TView>(params Guid[] ids) where TView : new();

		/// <summary>
		/// Gets a collection of accounts matching a given criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		Task<ICollection<TAccount>> FindAccountsAsync(AccountCriteria criteria);

		/// <summary>
		/// Gets a collection of accounts matching a given criteria.
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		Task<ICollection<TView>> FindAccountsAsync<TView>(AccountCriteria criteria) where TView : new();
	}
}
