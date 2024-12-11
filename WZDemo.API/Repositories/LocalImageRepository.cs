using WZDemo.API.Data;
using WZDemo.API.Models.Domain;

namespace WZDemo.API.Repositories
{
	public class LocalImageRepository : IImageRepository
	{
		private readonly IWebHostEnvironment webHostEnvironment;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly WZDemoDBContext dbContext;

		public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
			IHttpContextAccessor httpContextAccessor,
			WZDemoDBContext dbContext)
		{
			this.webHostEnvironment = webHostEnvironment;
			this.httpContextAccessor = httpContextAccessor;
			this.dbContext = dbContext;
		}
		/// <summary>
		/// 將上傳的圖像文件保存到本地文件系統
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public async Task<Image> Upload(Image image)
		{

			// 生成本地文件路徑
			var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images",
				$"{image.FileName}{image.FileExtension}");

			// 上傳圖像到本地路徑
			using var stream = new FileStream(localFilePath, FileMode.Create);
			await image.File.CopyToAsync(stream);

			// 生成圖像的 URL 路徑
			// https://localhost:7082/images/image.jpg
			var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
			image.FilePath = urlFilePath;

			// 將圖像信息保存到數據庫
			await dbContext.Images.AddAsync(image);
			await dbContext.SaveChangesAsync();

			return image;




		}
	}
}
