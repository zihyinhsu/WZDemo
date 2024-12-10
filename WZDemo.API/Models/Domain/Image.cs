using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace WZDemo.API.Models.Domain
{
	public class Image
	{
		public Guid Id { get; set; }
		[NotMapped]
		public IFormFile File { get; set; }


	}
}
