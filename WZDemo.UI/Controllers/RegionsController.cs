using Microsoft.AspNetCore.Mvc;
using WZDemo.UI.Models.DTO;

namespace WZDemo.UI.Controllers
{
	public class RegionsController : Controller
	{
		private readonly IHttpClientFactory httpClientFactory;

		public RegionsController(IHttpClientFactory httpClientFactory)
		{
			this.httpClientFactory = httpClientFactory;
		}

		public async Task<IActionResult> Index()
		{

			List<RegionDto> response = new List<RegionDto>();
			try
			{
				// 取得資料
				var client = httpClientFactory.CreateClient();

				var httpResponsemessage = await client.GetAsync("https://localhost:7082/api/Regions");

				httpResponsemessage.EnsureSuccessStatusCode(); // 確認是否成功

				response.AddRange(await httpResponsemessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>()); // 讀取內容
			}
			catch (Exception ex)
			{
				// Log the exception

			}

			return View(response);
		}
	}
}
