using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Silk.Web.Core.Abstractions;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Silk.Mapping;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Silk.Web.Core.TagHelpers
{
	[HtmlTargetElement("sitemenu", TagStructure = TagStructure.WithoutEndTag)]
	public class SiteMenuTagHelper : TagHelper
	{
		private readonly ICompositeViewEngine _viewEngine;
		private readonly IWebContext _webContext;
		private readonly IMenuBuilder _menuBuilder;
		private readonly IMapper _mapper;
		private readonly IModelMetadataProvider _modelMetadataProvider;

		public IMenuItem Menu { get; set; }
		public string MenuName { get; set; }

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public SiteMenuTagHelper(IWebContext webContext, ICompositeViewEngine viewEngine,
			IMenuBuilder menuBuilder, IMapper mapper, IModelMetadataProvider modelMetadataProvider)
		{
			_webContext = webContext;
			_viewEngine = viewEngine;
			_menuBuilder = menuBuilder;
			_mapper = mapper;
			_modelMetadataProvider = modelMetadataProvider;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var menu = Menu;
			if (menu == null)
				menu = await _menuBuilder.GetMenuAsync(MenuName).ConfigureAwait(false);

			output.TagName = "";
			var globalLayoutName = ((RazorView)ViewContext.View).RazorPage.Layout;

			if (menu.Menu == null)
			{
				var menuLayoutView = _viewEngine.FindViewCascade(ViewContext, false, menu.MenuName,
					$"{globalLayoutName}/Menus", "Default/Menus");
				if (!menuLayoutView.Success)
					throw new System.Exception($"Could not locate menu layout '{globalLayoutName}/Menus/{menu.MenuName}'");

				//var viewData = new ViewDataDictionary(ViewContext.ViewData);
				var viewData = new ViewDataDictionary(_modelMetadataProvider, new ModelStateDictionary());
				viewData.Model = menu;

				var viewContext = new ViewContext(ViewContext, ViewContext.View, viewData, ViewContext.TempData,
					ViewContext.Writer, new HtmlHelperOptions());

				await menuLayoutView.View.RenderAsync(viewContext);
			}
			else
			{
				var topMenu = menu.GetTopMenu();
				var viewName = menu.GetType().Name;
				if (viewName.EndsWith("MenuItem"))
					viewName = viewName.Substring(0, viewName.Length - 8);
				else if (viewName.EndsWith("Menu"))
					viewName = viewName.Substring(0, viewName.Length - 4);

				var view = _viewEngine.FindViewCascade(ViewContext, false, viewName,
					$"{globalLayoutName}/Menus/{topMenu.MenuName}", $"{globalLayoutName}/Menus/Default",
					$"Default/Menus/{topMenu.MenuName}", "Default/Menus/Default");
				if (!view.Success)
					throw new System.Exception($"Could not locate view for menu '{menu.MenuName}'");

				//var viewData = new ViewDataDictionary(ViewContext.ViewData);
				var viewData = new ViewDataDictionary(_modelMetadataProvider, new ModelStateDictionary());
				viewData.Model = menu;

				var viewContext = new ViewContext(ViewContext, ViewContext.View, viewData, ViewContext.TempData,
					ViewContext.Writer, new HtmlHelperOptions());

				await view.View.RenderAsync(viewContext);
			}
		}
	}
}
