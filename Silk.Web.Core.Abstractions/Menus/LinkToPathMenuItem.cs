namespace Silk.Web.Core.Abstractions.Menus
{
	public class LinkToPathMenuItem : IMenuItem
	{
		public LinkToPathMenuItem(string displayText, string linkPath)
		{
			MenuTitle = displayText;
			MenuName = displayText;
			Path = linkPath;
		}

		public string Path { get; }

		public string MenuName { get; }

		public string MenuTitle { get; }

		public IMenu Menu { get; set; }
	}
}
