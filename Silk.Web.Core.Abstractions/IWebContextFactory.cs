using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface IWebContextFactory
	{
		Task<IWebContext> CreateContextAsync();
	}
}
