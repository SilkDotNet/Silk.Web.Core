using Microsoft.Extensions.Logging;
using Silk.Web.Core.Components;
using Silk.Web.Core.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Silk.Data.SQL.Providers;
using Silk.Data.SQL.ORM;

namespace Silk.Web.Core.Installation
{
	/// <summary>
	/// Component to handle installation of Silk components and services.
	/// </summary>
	public class InstallerComponent : ComponentBase
	{
		private ILogger<InstallerComponent> _logger;

		[Startup]
		private async Task InstallerStartup(ILogger<InstallerComponent> logger, IDataProvider database,
			IAccountRepository<UserAccount> userAccounts, IServiceProvider serviceProvider,
			IEnumerable<ITokenGenerator> tokenGenerators, DataDomain dataDomain,
			ICredentialsManager<UsernamePasswordCredentials, UserAccount> credentialsChecker,
			IAccountRolesRepository roles)
		{
			_logger = logger;
			using (logger.BeginScope("Installation"))
			{
				await CreateSchema(database, serviceProvider, dataDomain)
					.ConfigureAwait(false);
				await CreateSuperUser(userAccounts, tokenGenerators, credentialsChecker, roles)
					.ConfigureAwait(false);
			}
		}

		private async Task CreateSchema(IDataProvider database, IServiceProvider serviceProvider,
			DataDomain dataDomain)
		{
			using (_logger.BeginScope("Database creation"))
			{
				foreach (var table in dataDomain.DataModels.SelectMany(q => q.Schema.Tables))
				{
					if (await table.ExistsAsync(database)
						.ConfigureAwait(false))
					{
						_logger.LogWarning($"'{table.TableName}' table exists, dropping.");
						await table.DropAsync(database)
							.ConfigureAwait(false);
					}
					_logger.LogInformation($"'{table.TableName}' creating table.");
					await table.CreateAsync(database)
						.ConfigureAwait(false);
				}
			}
		}

		private async Task CreateSuperUser(IAccountRepository<UserAccount> userAccounts,
			IEnumerable<ITokenGenerator> tokenGenerators,
			ICredentialsManager<UsernamePasswordCredentials, UserAccount> credentialsManager,
			IAccountRolesRepository roles)
		{
			var tokenGenerator = tokenGenerators.FirstOrDefault(q => q.IsCryptographicallySecure);
			if (tokenGenerator == null)
				tokenGenerator = tokenGenerators.FirstOrDefault();

			if (tokenGenerator == null)
				throw new InvalidOperationException("A token generator needs to be registered.");

			_logger.LogInformation("Creating super user account");
			var superUser = new UserAccount
			{
				DisplayName = "Super User",
				Username = "superuser"
			};
			await userAccounts.CreateAccountsAsync(superUser)
				.ConfigureAwait(false);
			var password = tokenGenerator.GenerateToken(6);
			await credentialsManager
				.SetAccountCredentialsAsync(superUser, new UsernamePasswordCredentials(superUser.Username, password))
				.ConfigureAwait(false);
			await roles.SetAccountRolesAsync(superUser, new[] { CoreRoles.SuperUserRole });
			_logger.LogCritical($"New super user account password: {password}");
		}
	}
}
