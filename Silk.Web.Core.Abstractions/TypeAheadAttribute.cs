using System;

namespace Silk.Web.Core.Abstractions
{
	[AttributeUsage(AttributeTargets.Property)]
	public class TypeAheadAttribute : Attribute
	{
		public Type RemoteQueryController { get; set; }
		public string RemoteQueryAction { get; set; }
	}
}
