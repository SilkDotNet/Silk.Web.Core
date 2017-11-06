using System.Threading.Tasks;
using Silk.Data.SQL.Providers;
using Silk.Data.SQL.ORM;
using System;
using System.Linq;

namespace Silk.Web.Core.Persistence
{
	public class DbUserPasswordCredentialsManager<TAccount> :
		DbRepositoryBase<DbUserPasswordCredentialsManager<TAccount>.LoginNamePasswordDomainModel, DbUserPasswordCredentialsManager<TAccount>.LoginNamePasswordDomainModel>,
		ICredentialsManager<UsernamePasswordCredentials, TAccount>
		where TAccount : UserAccount, new()
	{
		private readonly IAccountRepository<TAccount> _accounts;

		public DbUserPasswordCredentialsManager(IDataProvider database, IAccountRepository<TAccount> accounts)
			: base(database)
		{
			_accounts = accounts;
		}

		public async Task<TAccount> GetAccountByCredentialsAsync(UsernamePasswordCredentials credentials)
		{
			if (string.IsNullOrEmpty(credentials.UsernameOrEmail) ||
				string.IsNullOrEmpty(credentials.Password))
				return null;

			var normalizedName = credentials.UsernameOrEmail.ToLowerInvariant();
			var storedCredentials = (await DataModel
				.Select(
					where: DataModel.Where(q => q.Email == normalizedName || q.Username == normalizedName),
					limit: 1
					)
				.ExecuteAsync(Database)
				.ConfigureAwait(false))
				.FirstOrDefault();
			if (storedCredentials == null)
				return null;

			if (!BCrypt.Net.BCrypt.Verify(credentials.Password, storedCredentials.Password))
				return null;

			return await _accounts.GetAccountByIdAsync(storedCredentials.AccountId)
				.ConfigureAwait(false);
		}

		public Task SetAccountCredentialsAsync(TAccount account, UsernamePasswordCredentials credentials)
		{
			var password = BCrypt.Net.BCrypt.HashPassword(credentials.Password);
			return DataModel.Insert(new LoginNamePasswordDomainModel
			{
				AccountId = account.Id,
				Username = account.Username.ToLowerInvariant(),
				Email = account.EmailAddress?.ToLowerInvariant(),
				Password = password
			})
			.ExecuteAsync(Database);
		}

		public class LoginNamePasswordDomainModel
		{
			[PrimaryKey]
			public Guid AccountId { get; set; }
			public string Username { get; set; }
			public string Email { get; set; }
			[DataLength(128)]
			public string Password { get; set; }
		}
	}
}
