using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Silk.Web.Core.Data;
using Silk.Data.SQL.ORM.Modelling;

namespace Silk.Web.Core.Persistence
{
	public class DbAccountRolesRepository :
		DbRepositoryBase<DbAccountRolesRepository.AccountRole>,
		IAccountRolesRepository
	{
		public DbAccountRolesRepository(IDatabase<AccountRole> database) : base(database)
		{
		}

		protected override void CustomizeModel(ModelCustomizer<AccountRole> modelCustomizer)
		{
			modelCustomizer.For(q => q.AccountId).IsPrimaryKey();
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
			return Database
				.Delete(where: DataModel.Where(q => q.AccountId == account.Id))
				.Insert(roles.Select(role => new AccountRole { AccountId = account.Id, Role = role }).ToArray())
				.AsTransaction()
				.ExecuteAsync();
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
