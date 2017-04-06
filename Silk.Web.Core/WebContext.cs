using Silk.Web.Core.Abstractions;
using System;

namespace Silk.Web.Core
{
	public class WebContext : IWebContext
	{
		private readonly Func<Dashboard> _getDashboardDelegate;
		private Dashboard _dashboard;

		public Site Site { get; }
		public Dashboard Dashboard
		{
			get
			{
				if (_dashboard == null)
				{
					_dashboard = _getDashboardDelegate();
				}
				return _dashboard;
			}
		}

		public WebContext(Site site, Func<Dashboard> getDashboardDelegate)
		{
			Site = site;
			_getDashboardDelegate = getDashboardDelegate;
		}
	}
}
