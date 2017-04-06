using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface ISiteResolver
	{
		Site ResolveCurrentSite();
		Task<Site> ResolveCurrentSiteAsync();
	}
}
