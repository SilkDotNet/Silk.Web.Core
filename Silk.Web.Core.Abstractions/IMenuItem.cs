namespace Silk.Web.Core.Abstractions
{
	public interface IMenuItem
	{
		IMenu Menu { get; set; }

		string MenuName { get; }
		string MenuTitle { get; }
	}
}
