using Microsoft.AspNetCore.Http;
using Silk.Web.Core.Abstractions;
using System.Threading.Tasks;

namespace Silk.Web.Core
{
	internal class WebCoreMiddleware
	{
		private readonly RequestDelegate _next;

		public WebCoreMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, IWebContextFactory webContextFactory, WebContextState webContextState)
		{
			webContextState.Context = await webContextFactory.CreateContextAsync().ConfigureAwait(false);
			await _next.Invoke(context).ConfigureAwait(false);
		}
	}
}
