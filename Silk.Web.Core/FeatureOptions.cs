using System.Collections.Generic;

namespace Silk.Web.Core
{
	public class FeatureOptions
	{
		private readonly Dictionary<string, FeatureConfig> _featureConfigs = new Dictionary<string, FeatureConfig>();

		public FeatureConfig this[string featureName]
		{
			get
			{
				if (_featureConfigs.TryGetValue(featureName, out var config))
					return config;
				config = new FeatureConfig(featureName);
				this[featureName] = config;
				return config;
			}
			set
			{
				_featureConfigs[featureName] = value;
			}
		}
	}
}
