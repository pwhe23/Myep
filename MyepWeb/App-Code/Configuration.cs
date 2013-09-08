
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Funq;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.OrmLite;

namespace Site
{
	public static class Configuration
	{
		public static void Initialize()
		{
			var container = Ioc.Init();
			ConfigureLogging();
			ConfigureRoutes(RouteTable.Routes);
			ConfigureDatabase(container);
			Database.Configure(container);
			Database.Create(container);
		}

		public static void ConfigureLogging()
		{
			LogManager.LogFactory = new DebugLogFactory();
		}

		public static void ConfigureRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{action}/{id}",
				defaults: new { controller = "App", action = "Index", id = UrlParameter.Optional }
			);
		}

		public static void ConfigureDatabase(Container container)
		{
			var cs = ConfigurationManager.ConnectionStrings["SiteDb"].ConnectionString;

			container.Register<IDbConnectionFactory>(
				new OrmLiteConnectionFactory(cs, true, SqlServerDialect.Provider)
				{
					ConnectionFilter = x => new ProfiledDbConnection(x, Profiler.Current)
				}
			);
		}
	};

	public static class Ioc
	{
		private static Container _container;

		public static Container Init()
		{
			_container = new Container();
			return _container;
		}

		public static T Get<T>()
		{
			return _container.TryResolve<T>();
		}

		public static void Dispose()
		{
			if (_container != null) _container.Dispose();
		}
	};
}
