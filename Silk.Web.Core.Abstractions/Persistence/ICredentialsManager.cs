using System.Threading.Tasks;

namespace Silk.Web.Core.Persistence
{
	public interface ICredentialsManager
	{
	}

	public interface ICredentialsManager<TCredentials, TAccount>
		where TAccount : Account, new()
	{
		Task<TAccount> GetAccountByCredentialsAsync(TCredentials credentials);
		Task SetAccountCredentialsAsync(TAccount account, TCredentials credentials);
	}
}
