using System;
using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface ISiteRepository
	{
		Site GetSiteById(Guid siteId);
		Task<Site> GetSiteByIdAsync(Guid siteId);
		Site GetSiteByHostname(string hostname);
		Task<Site> GetSiteByHostnameAsync(string hostname);
	}
}
