using Microsoft.Extensions.Options;
using Silk.Web.Core.Abstractions;

namespace Silk.Web.Core
{
	internal class WebOptionsConfigure : ConfigureOptions<WebOptions>
	{
		public WebOptionsConfigure() : base(SetDefaults) { }

		private static void SetDefaults(WebOptions options)
		{
		}
	}
}
