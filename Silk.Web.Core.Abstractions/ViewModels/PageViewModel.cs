namespace Silk.Web.Core.Abstractions.ViewModels
{
	public class PageViewModel
	{
		public string Title { get; set; } = "";
		public string SiteName { get; set; } = "";
		public string CanonicalUrl { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
	}
}
