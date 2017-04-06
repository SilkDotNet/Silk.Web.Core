using System;

namespace Silk.Web.Core.Abstractions
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class GeneratedDbTableAttribute : Attribute
	{
	}
}
