using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreApiSuppression.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class HelloWorldController : ControllerBase
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public HelloWorldController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var client = _httpClientFactory.CreateClient();
			await client.GetAsync("https://www.google.com");
			return Ok();
		}
	}
}
