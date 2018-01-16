using AngleSharp.Parser.Html;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Web.Core.Notifications
{
	public class TemplatedNotificationSendService<T> : INotificationSendService<T>
	{
		private readonly ILogger<TemplatedNotificationSendService<T>> _logger;
		private readonly INotificationSender[] _notificationSenders;
		private readonly INotificationTemplateEngine _templateEngine;

		public TemplatedNotificationSendService(ILogger<TemplatedNotificationSendService<T>> logger,
			IEnumerable<INotificationSender> notificationSenders,
			INotificationTemplateEngine templateEngine)
		{
			_logger = logger;
			_notificationSenders = notificationSenders.ToArray();
			_templateEngine = templateEngine;
		}

		public Task SendNotificationAsync(T notification, params UserAccount[] recipientAccounts)
		{
			try
			{
				return SendNotificationAsyncImpl(notification, recipientAccounts);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to send notification");
				throw;
			}
		}

		public async Task SendNotificationAsyncImpl(T notification, UserAccount[] recipientAccounts)
		{
			if (_notificationSenders == null || _notificationSenders.Length == 0)
				throw new InvalidOperationException("No notification senders registered.");

			foreach (var notificationSender in _notificationSenders)
			{
				var notificationText = await _templateEngine
					.RenderAsync(notification, notificationSender.ProviderName)
					.ConfigureAwait(false);
				var title = ExtractTitle(notificationText);
				await notificationSender
					.SendAsync(title, notificationText, recipientAccounts)
					.ConfigureAwait(false);
			}
		}

		private string ExtractTitle(string htmlDocument)
		{
			var parser = new HtmlParser();
			var document = parser.Parse(htmlDocument);
			return document.Title;
		}
	}
}
