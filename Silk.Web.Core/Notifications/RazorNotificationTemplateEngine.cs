using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;

namespace Silk.Web.Core.Notifications
{
	public class RazorNotificationTemplateEngine : INotificationTemplateEngine
	{
		private readonly ICompositeViewEngine _viewEngine;
		private readonly ActionContext _actionContext;
		private readonly IModelMetadataProvider _modelMetadataProvider;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly HttpContext _httpContext;

		public RazorNotificationTemplateEngine(ICompositeViewEngine viewEngine,
			IActionContextAccessor actionContextAccessor, IModelMetadataProvider modelMetadataProvider,
			ITempDataProvider tempDataProvider, IHttpContextAccessor httpContextAccessor)
		{
			_viewEngine = viewEngine;
			_actionContext = actionContextAccessor.ActionContext;
			_modelMetadataProvider = modelMetadataProvider;
			_tempDataProvider = tempDataProvider;
			_httpContext = httpContextAccessor.HttpContext;
		}

		public async Task<string> RenderAsync<TNotification>(TNotification notification, string templateDir)
		{
			var viewSearchResult = _viewEngine.FindView(_actionContext, $"Notifications/{templateDir}/{typeof(TNotification).Name}", false);
			if (!viewSearchResult.Success)
				return null;

			using (var textWriter = new StringWriter())
			{
				var viewContext = new ViewContext(_actionContext, viewSearchResult.View,
					new ViewDataDictionary(_modelMetadataProvider, _actionContext.ModelState),
					new TempDataDictionary(_httpContext, _tempDataProvider), textWriter,
					new HtmlHelperOptions());
				viewContext.ViewData.Model = notification;
				await viewSearchResult.View.RenderAsync(viewContext)
					.ConfigureAwait(false);
				return textWriter.ToString();
			}
		}
	}
}
