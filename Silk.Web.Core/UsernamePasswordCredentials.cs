using Silk.Web.Core.Abstractions;

namespace Silk.Web.Core
{
	public class UsernamePasswordCredentials : IAccountCredentials
	{
		public string UsernameOrEmail { get; protected set; }
		public string Password { get; protected set; }

		public UsernamePasswordCredentials(string usernameOrEmail, string password)
		{
			UsernameOrEmail = usernameOrEmail;
			Password = password;
		}

		public virtual bool CompareCredentials(IAccountCredentials credentials)
		{
			if (credentials is UsernamePasswordCredentials userPassCredentials)
			{
				return userPassCredentials.UsernameOrEmail.ToLowerInvariant() ==
					UsernameOrEmail.ToLowerInvariant() &&
					userPassCredentials.Password == Password;
			}
			return false;
		}
	}
}
