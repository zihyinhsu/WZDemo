using System.Net;
using WZDemo.API.Models.Domain;

namespace WZDemo.API.Repositories
{
	public interface IImageRepository
	{
		Task<Image> Upload(Image image);
	}
}
