using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AspNetCoreApiSuppression
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOpenTelemetryTracing((builder) =>
			{
				builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
					.AddService(nameof(AspNetCoreApiSuppression)));

				builder.AddAspNetCoreInstrumentation(options =>
				{
					options.Filter = httpContext =>
					{
						if (HttpContextHelper.ShouldFilter(httpContext))
						{
							return false;
						}
						return true;
					};
				});
				builder.AddHttpClientInstrumentation();

				builder.SetSampler(new AlwaysOnSampler());
				builder.AddConsoleExporter();
				builder.AddOtlpExporter();
			});

			services.AddHttpClient();
			services.AddControllers();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseMiddleware<SpanSuppressionMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
