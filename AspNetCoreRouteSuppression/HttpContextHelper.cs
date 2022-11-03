using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;

namespace AspNetCoreApiSuppression
{
	public static class HttpContextHelper
	{
		public static bool ShouldFilter(HttpContext context)
		{
			if (context.Request.Path.StartsWithSegments("/status", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}
	}
}