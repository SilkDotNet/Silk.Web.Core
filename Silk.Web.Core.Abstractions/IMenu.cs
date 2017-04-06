namespace Silk.Web.Core.Abstractions
{
	public interface IMenu : IMenuItem
	{
		IMenuItem[] MenuItems { get; }

		void AddMenuItem(IMenuItem menuItem);
		void RemoveMenuItem(IMenuItem menuItem);
	}
}
