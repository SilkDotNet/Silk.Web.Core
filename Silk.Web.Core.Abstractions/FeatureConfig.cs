namespace Silk.Web.Core
{
	public class FeatureConfig
	{
		public string FeatureName { get; }
		public bool IsEnabled { get; set; }
		public string RouteUri { get; set; }

		public FeatureConfig(string featureName)
		{
			FeatureName = featureName;
		}
	}
}
