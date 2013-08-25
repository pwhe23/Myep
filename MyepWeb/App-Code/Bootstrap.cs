
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Funq;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.OrmLite;
using ServiceStack.Razor;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;

namespace Site
{
	public class AppHost : AppHostBase
	{
		public AppHost() : base("Myep", typeof(AppHost).Assembly) { }

		public override void Configure(Container container)
		{
			ConfigureLogging();
			ConfigurePlugins(Plugins);
			ConfigureRoutes(SetConfig);
			ConfigureDatabase(container);
			//AddAuthentication(container, Plugins);
		}

		private static void ConfigureLogging()
		{
			LogManager.LogFactory = new DebugLogFactory();
		}

		private static void ConfigurePlugins(List<IPlugin> plugins)
		{
			plugins.Add(new RazorFormat());
		}

		private static void ConfigureRoutes(Action<EndpointHostConfig> setConfig)
		{
			setConfig(new EndpointHostConfig
			{
				CustomHttpHandlers = {
					{ HttpStatusCode.NotFound, new RazorHandler("/notfound") },
					{ HttpStatusCode.Unauthorized, new RazorHandler("/login") },
				}
			});
		}

		private static void ConfigureDatabase(Container container)
		{
			var cs = ConfigurationManager.ConnectionStrings["SiteDb"].ConnectionString;

			container.Register<IDbConnectionFactory>(
				new OrmLiteConnectionFactory(cs, true, SqlServerDialect.Provider)
			);
		}

		private static void AddAuthentication(Container container, List<IPlugin> plugins)
		{
			container.Register<IUserAuthRepository>(c => new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

			var authRepo = (OrmLiteAuthRepository)container.Resolve<IUserAuthRepository>();
			authRepo.CreateMissingTables();

			plugins.Add(new AuthFeature(() =>
				new AuthUserSession(),
				new IAuthProvider[] { new CredentialsAuthProvider(), }
			));
		}
	};
}
