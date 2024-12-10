using System.ComponentModel.DataAnnotations;

namespace WZDemo.API.Models.DTOs
{
	public class RegisterRequestDto
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		public string UserName { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public string[] Roles { get; set; }
	}
}
