using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WZDemo.API.Repositories
{
	public class TokenRepository : ITokenRepository
	{
		private readonly IConfiguration configuration;

		public TokenRepository(IConfiguration configuration)
		{
			this.configuration = configuration;
		}
		public string CeateJwtToken(IdentityUser user, List<string> roles)
		{
			// 創建一個包含用戶聲明（Claims）的列表，這些聲明將用於生成 JWT
			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.Email, user.Email));

			foreach (var role in roles) {
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			// 創建一個對稱式加密金鑰
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

			// 創建一個簽名憑證
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				configuration["Jwt:Issuer"],
				configuration["Jwt:Audience"],
				claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
