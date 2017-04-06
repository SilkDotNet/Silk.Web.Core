using Microsoft.AspNetCore.Http;
using Silk.Web.Core.Abstractions;
using System.Threading.Tasks;

namespace Silk.Web.Core
{
	public class HostnameSiteResolver : ISiteResolver
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISiteRepository _siteRepository;

		public HostnameSiteResolver(ISiteRepository siteRepository, IHttpContextAccessor httpContextAccessor)
		{
			_siteRepository = siteRepository;
			_httpContextAccessor = httpContextAccessor;
		}

		public Site ResolveCurrentSite()
		{
			return _siteRepository.GetSiteByHostname(_httpContextAccessor.HttpContext.Request.Host.Host);
		}

		public Task<Site> ResolveCurrentSiteAsync()
		{
			return _siteRepository.GetSiteByHostnameAsync(_httpContextAccessor.HttpContext.Request.Host.Host);
		}
	}
}
