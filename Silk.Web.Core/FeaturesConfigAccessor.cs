using Microsoft.Extensions.Options;

namespace Silk.Web.Core
{
	public class FeaturesConfigAccessor : IFeaturesConfig
	{
		private readonly FeatureOptions _options;

		public FeaturesConfigAccessor(IOptions<FeatureOptions> featuresOptions)
		{
			_options = featuresOptions.Value;
		}

		public FeatureConfig GetFeatureConfig(string featureName)
		{
			return _options[featureName];
		}
	}
}
