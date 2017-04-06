using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Linq;

namespace Silk.Web.Core
{
	public static class ViewEngineExtensions
	{
		public static ViewEngineResult FindViewCascade(this IViewEngine viewEngine, ActionContext context, bool isMainPage, string viewName, params string[] viewDirectories)
		{
			return viewEngine.FindViewCascade(context, isMainPage, viewName, true, viewDirectories);
		}

		public static ViewEngineResult FindViewCascade(this IViewEngine viewEngine, ActionContext context, bool isMainPage, string viewName, bool searchForDefault, params string[] viewDirectories)
		{
			ViewEngineResult result = null;

			foreach(var viewDirectory in viewDirectories)
			{
				var viewPath = $"{viewDirectory}/{viewName}";
				result = viewEngine.FindView(context, viewPath, isMainPage);
				if (result.Success)
					return result;

				if (!searchForDefault)
					continue;

				viewPath = $"{viewDirectory}/Default";
				result = viewEngine.FindView(context, viewPath, isMainPage);
				if (result.Success)
					return result;
			}

			return result;
		}
	}
}
