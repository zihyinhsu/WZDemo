using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WZDemo.API.Models.Domain;
using WZDemo.API.Models.DTOs;
using WZDemo.API.Repositories;

namespace WZDemo.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ImagesController : ControllerBase
	{
		private readonly IImageRepository imageRepository;

		public ImagesController(IImageRepository imageRepository)
		{
			this.imageRepository = imageRepository;
		}

		[HttpPost]
		[Route("Upload")]
		public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
		{
			ValidateFileUpload(request);

			if (ModelState.IsValid) {

				// Convert DTO to Domain Model
				var imageDomainModel = new Image
				{
					File = request.File,
					FileExtension = Path.GetExtension(request.File.FileName),
					FileSizeInBytes = request.File.Length,
					FileName = request.FileName,
					FileDescription= request.FileDescription,
				};

				// User Repository to upload image
				await imageRepository.Upload(imageDomainModel);
				return Ok(imageDomainModel);
			}

			return BadRequest(ModelState);
		}

		private void ValidateFileUpload(ImageUploadRequestDto request)
		{
			var allowedxtension = new string[] { ".jpg", ".jpeg", ".png" };

			if (!allowedxtension.Contains(Path.GetExtension(request.File.FileName)))
			{
				ModelState.AddModelError("file", "檔案格式不正確");
			}

			// check if file size more than 10mb
			if (request.File.Length > 10485760) {
				ModelState.AddModelError("file", "檔案超過 10 mb，請重新上傳檔案");
			}
		}
	}
}
