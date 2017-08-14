using Silk.Web.Core.Components;

namespace Silk.Web.Core
{
	/// <summary>
	/// Silk web application.
	/// </summary>
	public interface IWebApplication
	{
		/// <summary>
		/// Get an array of all loaded components.
		/// </summary>
		LoadedComponent[] Components { get; }

		/// <summary>
		/// Gets an ITypeLocator that can find types in the web application.
		/// </summary>
		ITypeLocator TypeLocator { get; }
	}
}
