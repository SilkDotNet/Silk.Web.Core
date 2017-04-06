using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Routing;
using Silk.Web.Core.ViewModels;

namespace Silk.Web.Core.TagHelpers
{
	[HtmlTargetElement("form")]
	public class FormTagHelper : TagHelper
	{
		private readonly IModelMetadataProvider _modelMetadataProvider;
		private readonly ICompositeViewEngine _viewEngine;
		private IDictionary<string, string> _routeValues;
		private readonly IUrlHelperFactory _urlHelperFactory;

		private string _setEncType;

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public object Model { get; set; }
		public string ResetText { get; set; }
		public string SubmitText { get; set; } = "Submit";

		public string CancelText { get; set; }
		public string CancelController { get; set; }
		public string CancelAction { get; set; }
		public IDictionary<string, string> CancelRouteValues
		{
			get
			{
				if (_routeValues == null)
				{
					_routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				}

				return _routeValues;
			}
			set
			{
				_routeValues = value;
			}
		}

		public FormTagHelper(IModelMetadataProvider modelMetadataProvider,
			ICompositeViewEngine viewEngine, IUrlHelperFactory urlHelperFactory)
		{
			_modelMetadataProvider = modelMetadataProvider;
			_viewEngine = viewEngine;
			_urlHelperFactory = urlHelperFactory;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			if (Model == null)
				return;
			var modelMetadata = _modelMetadataProvider.GetMetadataForType(Model.GetType());
			foreach(var property in modelMetadata.Properties)
			{
				await RenderField(property, output, modelMetadata).ConfigureAwait(false);
			}

			if (!string.IsNullOrEmpty(_setEncType))
			{
				output.Attributes.SetAttribute("enctype", _setEncType);
			}

			if (!string.IsNullOrEmpty(SubmitText))
			{
				await RenderView("Submit", SubmitText, output).ConfigureAwait(false);
			}

			if (!string.IsNullOrEmpty(ResetText))
			{
				await RenderView("Reset", SubmitText, output).ConfigureAwait(false);
			}

			if (!string.IsNullOrEmpty(CancelText))
			{
				var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
				var url = urlHelper.Action(new UrlActionContext
				{
					Controller = CancelController,
					Action = CancelAction,
					Values = CancelRouteValues
				});
				await RenderView("Cancel", new CancelFormViewModel
				{
					Url = url,
					Text = CancelText
				}, output);
			}
		}

		private async Task RenderField(ModelMetadata propertyMetadata, TagHelperOutput output, ModelMetadata modelMetadata)
		{
			if (!propertyMetadata.ShowForEdit)
				return;

			if (propertyMetadata.GetHtmlInputType() == "file")
				_setEncType = "multipart/form-data";

			var value = propertyMetadata.PropertyGetter != null ? propertyMetadata.PropertyGetter(Model) : null;
			await RenderView(propertyMetadata.DataTypeName ?? propertyMetadata.ModelType.Name,
				propertyMetadata, output, value, modelMetadata).ConfigureAwait(false);
		}

		private async Task RenderView(string viewName, object model, TagHelperOutput output, object value = null,
			ModelMetadata modelMetadata = null)
		{
			var view = _viewEngine.FindViewCascade(ViewContext, false, viewName, "Forms/Controls");
			if (!view.Success)
				throw new Exception("Form component view not found.");

			var viewData = new ViewDataDictionary(_modelMetadataProvider, ViewContext.ModelState);
			viewData.Model = model;
			viewData["value"] = value;
			viewData["model"] = Model;
			viewData["modelMetadata"] = modelMetadata;

			using (var writer = new StringWriter())
			{
				var viewContext = new ViewContext(ViewContext, ViewContext.View, viewData, ViewContext.TempData,
					writer, new HtmlHelperOptions());

				await view.View.RenderAsync(viewContext).ConfigureAwait(false);

				output.Content.AppendHtml(writer.ToString());
			}
		}
	}
}
