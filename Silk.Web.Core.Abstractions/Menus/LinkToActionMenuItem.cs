namespace Silk.Web.Core.Abstractions.Menus
{
	public class LinkToActionMenuItem : IMenuItem
	{
		public string MenuName { get; }

		public string MenuTitle { get; }

		public IMenu Menu { get; set; }

		public string ControllerName { get; }

		public string ActionName { get; }

		public LinkToActionMenuItem(string title,
			string controllerName, string actionName)
		{
			MenuTitle = title;
			MenuName = title;
			ControllerName = controllerName;
			ActionName = actionName;
		}
	}
}
