namespace Silk.Web.Core.Abstractions
{
	public interface IWebContext
	{
		Site Site { get; }
		Dashboard Dashboard { get; }
	}
}
