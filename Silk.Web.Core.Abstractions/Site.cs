using System;

namespace Silk.Web.Core.Abstractions
{
	public class Site
	{
		public Guid SiteId { get; private set; }
		public string SiteName { get; set; }
		public string TagLine { get; set; }
		public Uri CanonicalUrl { get; set; }

		private Site() { } //  ctor for mapping use

		public Site(string siteName, string tagLine, Uri canonicalUrl) :
			this(siteName, tagLine, canonicalUrl, Guid.Empty)
		{
		}

		public Site(string siteName, string tagLine, string canonicalUrl) :
			this(siteName, tagLine, new Uri(canonicalUrl), Guid.Empty)
		{
		}

		public Site(string siteName, string tagLine, string canonicalUrl, Guid siteId) :
			this(siteName, tagLine, new Uri(canonicalUrl), siteId)
		{
		}

		public Site(string siteName, string tagLine, Uri canonicalUrl, Guid siteId)
		{
			SiteName = siteName;
			TagLine = tagLine;
			CanonicalUrl = canonicalUrl;
			SiteId = siteId;
		}
	}
}
