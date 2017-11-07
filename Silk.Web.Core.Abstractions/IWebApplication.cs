using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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

		/// <summary>
		/// Add a view provider to the view engine.
		/// </summary>
		/// <param name="viewFileProvider"></param>
		void AddViewProvider(IFileProvider viewFileProvider);

		/// <summary>
		/// Creates a service scope that works with IScopedServiceProvider.
		/// </summary>
		/// <returns></returns>
		IServiceScope CreateServiceScope();
	}
}
