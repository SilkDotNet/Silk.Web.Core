using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface IAccountAccessor<TAccount> where TAccount : IAccount
	{
		TAccount GetCurrentAccount();
		Task<TAccount> GetCurrentAccountAsync();
	}
}
