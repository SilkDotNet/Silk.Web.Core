using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Silk.Data.SQL.Providers;
using System.Linq;

namespace Silk.Web.Core.Persistence
{
	public class DbAccountRolesRepository :
		DbRepositoryBase<DbAccountRolesRepository.AccountRole, DbAccountRolesRepository.AccountRole>,
		IAccountRolesRepository
	{
		public DbAccountRolesRepository(IDataProvider database) : base(database)
		{
		}

		public async Task<List<string>> GetAccountRolesAsync(IAccountIdentifier account)
		{
			return (await 
				SelectAsync<RoleOnlyView>(where: DataModel.Where(q => q.AccountId == account.Id))
				.ConfigureAwait(false))
				.Select(role => role.Role)
				.ToList();
		}

		public Task SetAccountRolesAsync(IAccountIdentifier account, IEnumerable<string> roles)
		{
			return DataModel
				.Delete(where: DataModel.Where(q => q.AccountId == account.Id))
				.Insert(roles.Select(role => new AccountRole { AccountId = account.Id, Role = role }).ToArray())
				.AsTransaction()
				.ExecuteAsync(Database);
		}

		public class AccountRole
		{
			public Guid AccountId { get; set; }
			public string Role { get; set; }
		}

		private class RoleOnlyView
		{
			public string Role { get; set; }
		}
	}
}
