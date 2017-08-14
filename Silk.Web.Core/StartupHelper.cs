using Silk.Web.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Silk.Web.Core
{
	/// <summary>
	/// Find and execute startup methods on components.
	/// </summary>
	internal class StartupHelper
	{
		public void FindAndExecute(IServiceProvider serviceProvider, IWebApplication webApplication)
		{
			foreach (var component in webApplication.Components)
			{
				var startupMethods = GetStartupMethods(component.Component.GetType());
				foreach (var method in startupMethods)
				{
					RunMethod(component.Component, method, serviceProvider);
				}
			}
		}

		private MethodInfo[] GetStartupMethods(Type componentType)
		{
			var methods = componentType.GetMethods(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
				);
			return methods
				.Where(q => q.GetCustomAttribute<StartupAttribute>() != null)
				.ToArray();
		}

		private void RunMethod(IComponent component, MethodInfo method, IServiceProvider serviceProvider)
		{
			var args = new List<object>();
			foreach (var parameter in method.GetParameters())
			{
				args.Add(serviceProvider.GetService(parameter.ParameterType));
			}
			if (method.ReturnType == typeof(Task))
			{
				var task = method.Invoke(component, args.ToArray()) as Task;
				task.ConfigureAwait(false).GetAwaiter().GetResult();
			}
			else
			{
				method.Invoke(component, args.ToArray());
			}
		}
	}
}
