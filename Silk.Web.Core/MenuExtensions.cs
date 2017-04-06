using Silk.Web.Core.Abstractions;

namespace Silk.Web.Core
{
	public static class MenuExtensions
	{
		public static IMenu GetTopMenu(this IMenuItem menuItem)
		{
			var current = menuItem;
			while (current.Menu != null)
				current = current.Menu;
			return current as IMenu;
		}
	}
}
