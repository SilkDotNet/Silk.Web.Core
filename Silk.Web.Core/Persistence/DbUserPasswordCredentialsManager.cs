using System.Threading.Tasks;
using System;
using System.Linq;
using Silk.Web.Core.Data;
using Silk.Data.SQL.ORM.Modelling;
using Silk.Data.SQL;

namespace Silk.Web.Core.Persistence
{
	public class DbUserPasswordCredentialsManager<TAccount> :
		DbRepositoryBase<DbUserPasswordCredentialsManager<TAccount>.LoginNamePasswordDomainModel>,
		ICredentialsManager<UsernamePasswordCredentials, TAccount>
		where TAccount : UserAccount, new()
	{
		private readonly IAccountRepository<TAccount> _accounts;

		public DbUserPasswordCredentialsManager(IDatabase<LoginNamePasswordDomainModel> database,
			IAccountRepository<TAccount> accounts)
			: base(database)
		{
			_accounts = accounts;
		}

		protected override void CustomizeModel(ModelCustomizer<LoginNamePasswordDomainModel> modelCustomizer)
		{
			modelCustomizer.For(q => q.AccountId).IsPrimaryKey();
			modelCustomizer.For(q => q.Password).SqlType(SqlDataType.Text(128));
		}

		public async Task<TAccount> GetAccountByCredentialsAsync(UsernamePasswordCredentials credentials)
		{
			if (string.IsNullOrEmpty(credentials.UsernameOrEmail) ||
				string.IsNullOrEmpty(credentials.Password))
				return null;

			var normalizedName = credentials.UsernameOrEmail.ToLowerInvariant();
			var storedCredentials = (await Database
				.Select(
					where: DataModel.Where(q => q.Email == normalizedName || q.Username == normalizedName),
					limit: 1
					)
				.ExecuteAsync()
				.ConfigureAwait(false))
				.FirstOrDefault();
			if (storedCredentials == null)
				return null;

			if (!BCrypt.Net.BCrypt.Verify(credentials.Password, storedCredentials.Password))
				return null;

			return (await _accounts.GetAccountsByIdAsync(storedCredentials.AccountId)
				.ConfigureAwait(false)).FirstOrDefault();
		}

		public Task SetAccountCredentialsAsync(TAccount account, UsernamePasswordCredentials credentials)
		{
			var password = BCrypt.Net.BCrypt.HashPassword(credentials.Password);
			return Database.Insert(new LoginNamePasswordDomainModel
			{
				AccountId = account.Id,
				Username = account.Username.ToLowerInvariant(),
				Email = account.EmailAddress?.ToLowerInvariant(),
				Password = password
			})
			.ExecuteAsync();
		}

		public class LoginNamePasswordDomainModel
		{
			public Guid AccountId { get; set; }
			public string Username { get; set; }
			public string Email { get; set; }
			public string Password { get; set; }
		}
	}
}
