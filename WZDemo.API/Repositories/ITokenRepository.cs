using Microsoft.AspNetCore.Identity;

namespace WZDemo.API.Repositories
{
	public interface ITokenRepository
	{
		string CeateJwtToken(IdentityUser user, List<string> roles); 
		
	}
}
