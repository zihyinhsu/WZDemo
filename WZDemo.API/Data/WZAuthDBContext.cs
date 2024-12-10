using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WZDemo.API.Data
{
	public class WZAuthDBContext : IdentityDbContext
	{
		public WZAuthDBContext(DbContextOptions<WZAuthDBContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			var readerRoleId = "263d25ea-6028-46db-b390-27f9089f51c6";
			var writerRoleId = "bb20df13-2509-41fb-84d8-934343023dcf";

			// Seeding Data
			var roles = new List<IdentityRole>
			{
				new IdentityRole
				{
					Id = readerRoleId,
					ConcurrencyStamp = readerRoleId,
					Name = "Reader",
					NormalizedName = "Reader".ToUpper()
				},
				new IdentityRole
				{
					Id = writerRoleId,
					ConcurrencyStamp = writerRoleId,
					Name = "Writer",
					NormalizedName = "Writer".ToUpper()
				},
			};

			builder.Entity<IdentityRole>().HasData(roles);
		}
	}
}
