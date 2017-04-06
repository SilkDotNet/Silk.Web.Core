using Silk.Web.Core.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Web.Core
{
	public class InMemorySiteRepository : ISiteRepository
	{
		private readonly IWebBuilder _webBuilder;

		public InMemorySiteRepository(IWebBuilder webBuilder)
		{
			_webBuilder = webBuilder;
		}

		public Site GetSiteByHostname(string hostname)
		{
			return _webBuilder.Sites.FirstOrDefault(q => q.CanonicalUrl.Host.ToLowerInvariant() == hostname.ToLowerInvariant());
		}

		public Task<Site> GetSiteByHostnameAsync(string hostname)
		{
			return Task.FromResult(
				GetSiteByHostname(hostname)
			);
		}

		public Site GetSiteById(Guid siteId)
		{
			return _webBuilder.Sites.FirstOrDefault(q => q.SiteId.Equals(siteId));
		}

		public Task<Site> GetSiteByIdAsync(Guid siteId)
		{
			return Task.FromResult(
				GetSiteById(siteId)
			);
		}
	}
}
