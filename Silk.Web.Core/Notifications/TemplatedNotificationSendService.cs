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

		public Task SendNotificationAsync(T notification)
		{
			try
			{
				return SendNotificationAsyncImpl(notification);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to send notification");
				throw;
			}
		}

		public async Task SendNotificationAsyncImpl(T notification)
		{
			if (_notificationSenders == null || _notificationSenders.Length == 0)
				throw new InvalidOperationException("No notification senders registered.");

			foreach (var notificationSender in _notificationSenders)
			{
				var notificationText = await _templateEngine
					.RenderAsync(notification, notificationSender.ProviderName)
					.ConfigureAwait(false);
				await notificationSender
					.SendAsync("", notificationText)
					.ConfigureAwait(false);
			}
		}
	}
}
