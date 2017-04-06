using System.Collections.Generic;
using Silk.Web.Core.Abstractions;
using Silk.Web.Core.Abstractions.Menus;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Silk.Web.Core.Menus
{
	public class DashboardMenuProvider : IMenuProvider
	{
		private readonly IEnumerable<Dashboard> _dashboards;
		private readonly ClaimsPrincipal _user;

		public DashboardMenuProvider(IDashboardProvider dashboardProvider,
			IHttpContextAccessor httpContextAccessor)
		{
			_dashboards = dashboardProvider.Dashboards;
			_user = httpContextAccessor.HttpContext.User;
		}

		public void BuildMenu(IMenu menu)
		{
			if (menu.MenuName != CoreMenus.DashboardAux)
				return;

			BuildMenu(menu, GetUserDashboards());
		}

		public async Task BuildMenuAsync(IMenu menu)
		{
			if (menu.MenuName != CoreMenus.DashboardAux)
				return;

			BuildMenu(menu, await GetUserDashboardsAsync());
		}

		private void BuildMenu(IMenu menu, List<Dashboard> dashboards)
		{
			if (dashboards.Count == 0)
				return;

			var dashboardMenu = new TopLevelMenu("Dashboards", "Dashboards");
			menu.AddMenuItem(dashboardMenu);

			foreach (var dashboard in dashboards)
			{
				dashboardMenu.AddMenuItem(new LinkToPathMenuItem(dashboard.Name, dashboard.Path));
			}
		}

		private List<Dashboard> GetUserDashboards()
		{
			var ret = new List<Dashboard>();
			foreach(var dashboard in _dashboards)
			{
				if (dashboard.AccessChecker.CanAccess(_user))
					ret.Add(dashboard);
			}
			return ret;
		}

		private async Task<List<Dashboard>> GetUserDashboardsAsync()
		{
			var ret = new List<Dashboard>();
			foreach (var dashboard in _dashboards)
			{
				if (await dashboard.AccessChecker.CanAccessAsync(_user).ConfigureAwait(false))
					ret.Add(dashboard);
			}
			return ret;
		}
	}
}