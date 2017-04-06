using Silk.Web.Core.Abstractions;
using Silk.Web.Core.Abstractions.Menus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Silk.Web.Core
{
	internal class MenuBuilder : IMenuBuilder
	{
		private readonly IEnumerable<IMenuProvider> _menuProviders;
		private readonly Dictionary<string, IMenu> _menuCache = new Dictionary<string, IMenu>();

		public MenuBuilder(IEnumerable<IMenuProvider> menuProviders)
		{
			_menuProviders = menuProviders;
		}

		public IMenu GetMenu(string menuName, bool forceRebuild = false)
		{
			IMenu menu = null;

			if (!forceRebuild && _menuCache.TryGetValue(menuName, out menu))
				return menu;

			menu = BuildMenu(menuName);
			_menuCache[menuName] = menu;

			return menu;
		}

		public async Task<IMenu> GetMenuAsync(string menuName, bool forceRebuild = false)
		{
			IMenu menu = null;

			if (!forceRebuild && _menuCache.TryGetValue(menuName, out menu))
				return menu;

			menu = await BuildMenuAsync(menuName).ConfigureAwait(false);
			_menuCache[menuName] = menu;

			return menu;
		}

		private IMenu BuildMenu(string menuName)
		{
			var menu = new TopLevelMenu(menuName, null);

			foreach (var menuProvider in _menuProviders)
			{
				menuProvider.BuildMenu(menu);
			}

			return menu;
		}

		private async Task<IMenu> BuildMenuAsync(string menuName)
		{
			var menu = new TopLevelMenu(menuName, null);

			foreach (var menuProvider in _menuProviders)
			{
				await menuProvider.BuildMenuAsync(menu).ConfigureAwait(false);
			}

			return menu;
		}
	}
}
