namespace Silk.Web.Core
{
	public interface IFeaturesConfig
	{
		FeatureConfig GetFeatureConfig(string featureName);
	}
}
