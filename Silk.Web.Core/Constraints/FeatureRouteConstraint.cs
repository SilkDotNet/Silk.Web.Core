using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Silk.Web.Core.Constraints
{
	public class FeatureRouteConstraint : IRouteConstraint
	{
		public string FeatureName { get; }
		public FeatureConfig Config { get; private set; }

		public FeatureRouteConstraint(string featureName)
		{
			FeatureName = featureName;
		}

		private void EnsureSettingsPopulated(IServiceProvider serviceProvider)
		{
			if (Config != null)
				return;

			var featuresConfig = serviceProvider.GetRequiredService<IFeaturesConfig>();
			Config = featuresConfig.GetFeatureConfig(FeatureName);
		}

		public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
		{
			EnsureSettingsPopulated(httpContext.RequestServices);
			if (!Config.IsEnabled)
				return false;
			if (values.TryGetValue(routeKey, out var value) && value != null && value is string str)
				return str == Config.RouteUri;
			return false;
		}
	}
}
