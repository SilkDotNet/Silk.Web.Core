namespace Silk.Web.Core.Components
{
	/// <summary>
	/// A component that has been loaded.
	/// </summary>
	public class LoadedComponent
	{
		/// <summary>
		/// Gets the component information.
		/// </summary>
		public ComponentInformation Information { get; }

		/// <summary>
		/// Gets the component instance.
		/// </summary>
		public IComponent Component { get; }

		public LoadedComponent(ComponentInformation componentInformation, IComponent component)
		{
			Information = componentInformation;
			Component = component;
		}
	}
}
