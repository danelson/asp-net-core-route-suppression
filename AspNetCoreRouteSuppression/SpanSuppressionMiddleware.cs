using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace AspNetCoreApiSuppression
{
	public class SpanSuppressionMiddleware
	{
		private readonly RequestDelegate _next;

		public SpanSuppressionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (HttpContextHelper.ShouldFilter(context))
			{
				using var _ = SuppressInstrumentationScope.Begin();
				await _next.Invoke(context);
			}
			else
			{
				await _next.Invoke(context);
			}
		}
	}
}