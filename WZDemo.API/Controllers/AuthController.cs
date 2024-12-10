using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WZDemo.API.Models.DTOs;
using WZDemo.API.Repositories;

namespace WZDemo.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<IdentityUser> userManager;
		private readonly ITokenRepository tokenRepository;

		public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
		{
			this.userManager = userManager;
			this.tokenRepository = tokenRepository;
		}
		// POST: api/Auth/Register
		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
		{

			var identityUser = new IdentityUser
			{
				UserName = registerRequestDto.UserName,
				Email = registerRequestDto.UserName
			};

			var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

			if (identityResult.Succeeded)
			{
				// Add roles to user
				if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
				{
					identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
					if (identityResult.Succeeded)
					{
						return Ok("註冊成功! 請登入帳號");
					}
				}
			}

			return BadRequest($"{identityResult.Errors}, 請聯繫系統管理員");
		}

		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
		{
			var user = await userManager.FindByEmailAsync(loginRequestDto.UserName);
			if (user != null)
			{
				var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
				if (checkPasswordResult)
				{
					var roles = await userManager.GetRolesAsync(user);
					if (roles != null)
					{
						// Create Token
						var jwtToken = tokenRepository.CeateJwtToken(user, roles.ToList());
						
						var response = new LoginResponseDto
						{
							JwtToken = jwtToken
						};

						return Ok(response);
					}
				}
			}

			return BadRequest("登入失敗");
		}
	}
}
