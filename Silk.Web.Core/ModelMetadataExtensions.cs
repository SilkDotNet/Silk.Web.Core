using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Silk.Web.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Web.Core
{
	public static class ModelMetadataExtensions
	{
		public static string[] GetErrors(this ModelMetadata modelMetadata, ModelStateDictionary modelStateDictionary)
		{
			var modelState = modelStateDictionary[modelMetadata.PropertyName];
			if (modelState == null || modelState.ValidationState == ModelValidationState.Valid)
				return new string[0];
			var ret = new List<string>();
			foreach (var modelError in modelState.Errors)
			{
				ret.Add(modelError.ErrorMessage);
			}
			return ret.ToArray();
		}

		public static bool IsTypeAhead(this ModelMetadata modelMetadata)
		{
			return modelMetadata.GetTypeAheadAttribute() != null;
		}

		public static TypeAheadAttribute GetTypeAheadAttribute(this ModelMetadata modelMetadata)
		{
			var defaultModelMetadata = modelMetadata as Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelMetadata;
			if (defaultModelMetadata == null)
				return null;
			return defaultModelMetadata.Attributes.Attributes.OfType<TypeAheadAttribute>()
				.FirstOrDefault();
		}

		public static string GetTypeAheadUrl(this ModelMetadata modelMetadata, IUrlHelper urlHelper)
		{
			var typeAheadAttr = modelMetadata.GetTypeAheadAttribute();
			if (typeAheadAttr == null)
				return "";
			return urlHelper.Action(typeAheadAttr.RemoteQueryAction,
				typeAheadAttr.RemoteQueryController.Name.ControllerName());
		}

		public static bool IsSelect(this ModelMetadata modelMetadata)
		{
			return modelMetadata.GetSelectListAttribute() != null;
		}

		public static SelectListItem[] GetSelectList(this ModelMetadata modelMetadata, object model, object parentMetadata)
		{
			var parentMetadataStaticTyped = parentMetadata as ModelMetadata;
			var selectListAttribute = modelMetadata.GetSelectListAttribute();
			var propertyName = selectListAttribute.ItemsPropertyName;
			return parentMetadataStaticTyped.Properties[propertyName].PropertyGetter(model) as SelectListItem[];
		}

		public static SelectListAttribute GetSelectListAttribute(this ModelMetadata modelMetadata)
		{
			var defaultModelMetadata = modelMetadata as Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelMetadata;
			if (defaultModelMetadata == null)
				return null;
			return defaultModelMetadata.Attributes.Attributes.OfType<SelectListAttribute>()
				.FirstOrDefault();
		}

		public static string GetHtmlInputType(this ModelMetadata modelMetadata)
		{
			var type = "text";
			var elementType = modelMetadata.ElementType;
			if (elementType == null)
				elementType = modelMetadata.ModelType;
			if (
				elementType == typeof(int) || elementType == typeof(uint) ||
				elementType == typeof(short) || elementType == typeof(ushort) ||
				elementType == typeof(long) || elementType == typeof(ulong) ||
				elementType == typeof(sbyte) || elementType == typeof(byte)
				)
				type = "number";
			if (elementType == typeof(IFormFile))
				type = "file";
			if (elementType == typeof(DateTime) || elementType == typeof(DateTime?))
				type = "date";
			return type;
		}
	}
}
