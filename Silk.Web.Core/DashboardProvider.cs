using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Silk.Web.Core.Abstractions;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Silk.Web.Core
{
	internal class DashboardProvider : IDashboardProvider
	{
		public IEnumerable<Dashboard> Dashboards { get; private set; }

		public DashboardProvider(ApplicationPartManager applicationManager, IUrlHelperFactory urlHelperFactory, 
			IActionContextAccessor actionContextAccessor, IAuthorizationService authService, IAuthorizationPolicyProvider policyProvider)
		{
			var feature = new ControllerFeature();
			applicationManager.PopulateFeature(feature);
			PopulateDashboards(feature.Controllers,
				urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext),
				authService, policyProvider);
		}

		private void PopulateDashboards(IEnumerable<TypeInfo> controllers, IUrlHelper urlHelper, IAuthorizationService authService,
			IAuthorizationPolicyProvider policyProvider)
		{
			var dashboards = new List<Dashboard>();
			foreach(var controller in controllers)
			{
				var dashboardAttr = controller.GetCustomAttribute<DashboardAttribute>();
				if (dashboardAttr == null)
					continue;
				var path = urlHelper.Action("Index", controller.Name.ControllerName());
				dashboards.Add(new Dashboard(dashboardAttr.Layout, dashboardAttr.Name, controller, path,
					new DashboardAccessChecker(controller, authService, policyProvider)));
			}
			Dashboards = dashboards;
		}

		private class DashboardAccessChecker : IDashboardAccessChecker
		{
			private readonly List<AccessChecker> _checkers = new List<AccessChecker>();
			private readonly IAuthorizationService _authService;

			public DashboardAccessChecker(TypeInfo controllerTypeInfo, IAuthorizationService authService,
				IAuthorizationPolicyProvider policyProvider)
			{
				_authService = authService;
				var authAttributes = controllerTypeInfo.GetCustomAttributes<AuthorizeAttribute>().ToArray();
				foreach(var authAttr in authAttributes)
				{
					var policyBuilder = new AuthorizationPolicyBuilder();
					if (string.IsNullOrEmpty(authAttr.Roles) &&
						string.IsNullOrEmpty(authAttr.Policy))
					{
						policyBuilder.RequireAuthenticatedUser();
					}
					else
					{
						if (!string.IsNullOrEmpty(authAttr.Roles))
						{
							policyBuilder.RequireRole(authAttr.Roles.Split(','));
						}
						if (!string.IsNullOrEmpty(authAttr.Policy))
						{
							var policy = policyProvider.GetPolicyAsync(authAttr.Policy).GetAwaiter().GetResult();
							if (policy != null)
								policyBuilder.Combine(policy);
						}
					}
					_checkers.Add(new PolicyChecker(policyBuilder.Build()));
				}
			}

			public bool CanAccess(ClaimsPrincipal user)
			{
				foreach(var checker in _checkers)
				{
					if (!checker.CanAccess(user, _authService))
						return false;
				}
				return true;
			}

			public async Task<bool> CanAccessAsync(ClaimsPrincipal user)
			{
				foreach (var checker in _checkers)
				{
					if (!await checker.CanAccessAsync(user, _authService).ConfigureAwait(false))
						return false;
				}
				return true;
			}
		}

		private abstract class AccessChecker
		{
			public abstract bool CanAccess(ClaimsPrincipal user, IAuthorizationService authorizationService);
			public abstract Task<bool> CanAccessAsync(ClaimsPrincipal user, IAuthorizationService authorizationService);
		}

		private class PolicyChecker : AccessChecker
		{
			public AuthorizationPolicy Policy { get; }

			public PolicyChecker(AuthorizationPolicy policy)
			{
				Policy = policy;
			}

			public override bool CanAccess(ClaimsPrincipal user, IAuthorizationService authorizationService)
			{
				return authorizationService.AuthorizeAsync(user, Policy).GetAwaiter().GetResult();
			}

			public override Task<bool> CanAccessAsync(ClaimsPrincipal user, IAuthorizationService authorizationService)
			{
				return authorizationService.AuthorizeAsync(user, Policy);
			}
		}
	}
}
