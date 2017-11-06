using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Silk.Web.Core
{
	public static class WebApplicationExtensions
	{
		/// <summary>
		/// Add a view provider to the view engine that provides access to embedded views.
		/// </summary>
		/// <param name="webApplication"></param>
		/// <param name="assembly"></param>
		public static void AddEmbeddedViewProvider(this IWebApplication webApplication, Assembly assembly)
		{
			webApplication.AddViewProvider(new EmbeddedFileProvider(assembly));
		}
	}
}
