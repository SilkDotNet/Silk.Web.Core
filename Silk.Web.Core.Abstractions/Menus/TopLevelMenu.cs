using System.Collections.Generic;

namespace Silk.Web.Core.Abstractions.Menus
{
	public class TopLevelMenu : IMenu
	{
		private List<IMenuItem> _menuItems = new List<IMenuItem>();

		public IMenu Menu { get; set; }

		public string MenuName { get; }

		public string MenuTitle { get; }

		public IMenuItem[] MenuItems => _menuItems.ToArray();

		public TopLevelMenu(string menuName, string menuTitle)
		{
			MenuName = menuName;
			MenuTitle = menuTitle;
		}

		public void AddMenuItem(IMenuItem menuItem)
		{
			_menuItems.Add(menuItem);
			menuItem.Menu = this;
		}

		public void RemoveMenuItem(IMenuItem menuItem)
		{
			_menuItems.Remove(menuItem);
			menuItem.Menu = null;
		}
	}
}
