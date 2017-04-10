using System;

namespace Silk.Web.Core.Abstractions
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ServiceInitializeAttribute : Attribute
	{
	}
}
