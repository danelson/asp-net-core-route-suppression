using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreApiSuppression.Controllers
{
	[ApiController]
	[Route("/status")]
	public class StatusController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public StatusController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet]
		[Route("live")]
		public async Task<IActionResult> IsLive()
		{
			var client = _httpClientFactory.CreateClient();
			await client.GetAsync("https://www.google.com");
			return Ok("live");
		}
	}
}
