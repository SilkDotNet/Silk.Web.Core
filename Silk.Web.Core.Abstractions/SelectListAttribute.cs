using System;

namespace Silk.Web.Core.Abstractions
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SelectListAttribute : Attribute
	{
		public SelectListAttribute(string itemsPropertyName)
		{
			ItemsPropertyName = itemsPropertyName;
		}

		public string ItemsPropertyName { get; private set; }
	}
}
