using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Silk.Web.Core.Abstractions
{
	public class DashboardAttribute : Attribute, IAuthorizationFilter
	{
		public string Layout { get; set; } = "_DashboardLayout";
		public string Name { get; set; } = "Dashboard";

		public void OnAuthorization(AuthorizationFilterContext context)
		{
		}
	}
}
