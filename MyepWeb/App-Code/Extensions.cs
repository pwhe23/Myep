
using System;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;

namespace Site
{
	public static class Ext
	{
		public static HttpResult Created(this Service service, Object model, string location)
		{
			return new HttpResult(model)
			{
				StatusCode = HttpStatusCode.Redirect,
				Headers = {
					{ HttpHeaders.Location, location }
				}
			};
		}
	};
}
