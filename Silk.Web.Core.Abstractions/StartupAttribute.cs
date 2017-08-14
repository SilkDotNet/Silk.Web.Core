using System;

namespace Silk.Web.Core
{
	/// <summary>
	/// Mark a method as a method to be executed when the webapplication starts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class StartupAttribute : Attribute
	{
	}
}
