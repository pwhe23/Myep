
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Funq;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.OrmLite;
using ServiceStack.WebHost.Endpoints;

namespace Site
{
	public static class Configuration
	{
		public static void ConfigureLogging()
		{
			LogManager.LogFactory = new DebugLogFactory();
		}

		public static void ConfigureContainer(Container container)
		{
			container.RegisterAutoWired<EmployersRepository>().ReusedWithin(ReuseScope.None);
			container.RegisterAutoWired<InternsRepository>().ReusedWithin(ReuseScope.None);
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

		public static void ConfigureRoutes(Action<EndpointHostConfig> setConfig)
		{
			setConfig(new EndpointHostConfig
			{
				GlobalResponseHeaders = {
					{ "Access-Control-Allow-Origin", "*" },
					{ "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
				},
			});
		}

		public static void ConfigureDatabase(Container container)
		{
			var cs = ConfigurationManager.ConnectionStrings["SiteDb"].ConnectionString;

			container.Register<IDbConnectionFactory>(
				new OrmLiteConnectionFactory(cs, true, SqlServerDialect.Provider)
			);
		}

		public static void CreateDatabase(Container container)
		{
			using (var db = container.Resolve<IDbConnectionFactory>().OpenDbConnection())
			{
				db.AlterTable<Employer>();
				db.AlterTable<Intern>();
				//db.InsertAll(SeedRockstars);
			}
		}
	};

	public class AppHost : AppHostBase
	{
		public AppHost() : base("Myep", typeof(AppHost).Assembly) { }

		public override void Configure(Container container)
		{
			Configuration.ConfigureLogging();
			Configuration.ConfigureRoutes(SetConfig);
			Configuration.ConfigureDatabase(container);
			Configuration.ConfigureContainer(container);
			Configuration.CreateDatabase(container);
		}
	};

	public static class Ioc
	{
		public static T Get<T>()
		{
			return EndpointHost.AppHost.TryResolve<T>();
		}
	};
}
