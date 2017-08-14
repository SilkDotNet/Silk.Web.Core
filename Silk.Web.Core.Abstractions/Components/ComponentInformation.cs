namespace Silk.Web.Core.Components
{
	/// <summary>
	/// Component information.
	/// </summary>
	public class ComponentInformation
	{
		/// <summary>
		/// Gets the component loader that can load the component.
		/// </summary>
		public IComponentLoader Loader { get; }

		public ComponentInformation(IComponentLoader loader)
		{
			Loader = loader;
		}
	}
}
