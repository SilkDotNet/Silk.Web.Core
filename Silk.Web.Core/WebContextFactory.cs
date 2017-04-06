using System.Threading.Tasks;
using Silk.Web.Core.Abstractions;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Silk.Web.Core
{
	public class WebContextFactory : IWebContextFactory
	{
		private readonly IEnumerable<ISiteResolver> _siteResolvers;
		private readonly IActionContextAccessor _actionContextAccessor;
		private IDashboardProvider _dashboardProvider;

		public WebContextFactory(IEnumerable<ISiteResolver> siteResolvers, IOptions<WebOptions> webOptions,
			IActionContextAccessor actionContextAccessor)
		{
			_siteResolvers = siteResolvers;
			_actionContextAccessor = actionContextAccessor;
		}

		public async Task<IWebContext> CreateContextAsync()
		{
			Site site = null;

			foreach(var siteResolver in _siteResolvers)
			{
				site = await siteResolver.ResolveCurrentSiteAsync().ConfigureAwait(false);
				if (site != null)
					break;
			}

			return new WebContext(site, GetDashboard);
		}

		private void EnsureDashboardProviderCreated()
		{
			if (_dashboardProvider == null)
			{
				_dashboardProvider = _actionContextAccessor.ActionContext.HttpContext.RequestServices.GetRequiredService<IDashboardProvider>();
			}
		}

		private Dashboard GetDashboard()
		{
			EnsureDashboardProviderCreated();

			Dashboard dashboard = null;
			var actionContext = _actionContextAccessor.ActionContext;
			if (actionContext?.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
			{
				dashboard = _dashboardProvider.Dashboards.FirstOrDefault(q => q.ControllerTypeInfo == controllerActionDescriptor.ControllerTypeInfo);
			}

			if (actionContext != null && actionContext.RouteData.Values.ContainsKey("dashboard"))
			{
				var path = $"/{actionContext.RouteData.Values["dashboard"]}";
				dashboard = _dashboardProvider.Dashboards.FirstOrDefault(q => q.Path == path);
			}

			if (actionContext != null && dashboard != null)
				actionContext.RouteData.Values["dashboard"] = dashboard.Path;

			return dashboard;
		}
	}
}
