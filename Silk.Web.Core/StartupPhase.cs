namespace Silk.Web.Core
{
	/// <summary>
	/// Phases of application startup.
	/// </summary>
	internal enum StartupPhase
	{
		HostBuilding,
		ComponentDiscovery,
		ComponentCreation,
		ComponentInitalization,
		ConfiguringServices,
		AddingMiddleware,
		Startup,
		Running
	}
}
