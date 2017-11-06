/**
 * Credit for this code goes to Jeff Putz
 * https://weblogs.asp.net/jeff/adding-a-bootstrap-css-class-for-validation-failure-in-asp-net-core
 */
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;

namespace Silk.Web.Core.TagHelpers
{
	[HtmlTargetElement("input", Attributes = ValidationForAttributeName + "," + ValidationErrorClassName)]
	[HtmlTargetElement("textarea", Attributes = ValidationForAttributeName + "," + ValidationErrorClassName)]
	[HtmlTargetElement("select", Attributes = ValidationForAttributeName + "," + ValidationErrorClassName)]
	[HtmlTargetElement("div", Attributes = ValidationForAttributeName + "," + ValidationErrorClassName)]
	public class ValidationClassTagHelper : TagHelper
	{
		private const string ValidationForAttributeName = "asp-for";
		private const string ValidationErrorClassName = "silk-validationerror-class";

		[HtmlAttributeName(ValidationForAttributeName)]
		public ModelExpression For { get; set; }

		[HtmlAttributeName(ValidationErrorClassName)]
		public string ValidationErrorClass { get; set; }

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			ModelStateEntry entry;
			ViewContext.ViewData.ModelState.TryGetValue(For.Name, out entry);
			if (entry == null || !entry.Errors.Any()) return;
			var tagBuilder = new TagBuilder(context.TagName);
			tagBuilder.AddCssClass(ValidationErrorClass);
			output.MergeAttributes(tagBuilder);
		}
	}
}
