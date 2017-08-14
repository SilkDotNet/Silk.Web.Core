namespace Silk.Web.Core.Components
{
	/// <summary>
	/// Handles loading of components.
	/// </summary>
	public interface IComponentLoader
	{
		/// <summary>
		/// Creates a new instance of a component.
		/// </summary>
		/// <returns></returns>
		IComponent CreateComponentInstance();
	}
}
