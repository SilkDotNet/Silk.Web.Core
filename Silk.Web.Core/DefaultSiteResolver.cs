using System.Threading.Tasks;
using Silk.Web.Core.Abstractions;

namespace Silk.Web.Core
{
	public class DefaultSiteResolver : ISiteResolver
	{
		private readonly IWebBuilder _webBuilder;

		public DefaultSiteResolver(IWebBuilder webBuilder)
		{
			_webBuilder = webBuilder;
		}

		public Site ResolveCurrentSite()
		{
			return _webBuilder.DefaultSite;
		}

		public Task<Site> ResolveCurrentSiteAsync()
		{
			return Task.FromResult(ResolveCurrentSite());
		}
	}
}
