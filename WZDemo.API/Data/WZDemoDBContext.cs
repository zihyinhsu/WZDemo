
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection.Emit;
using WZDemo.API.Models.Domain;

namespace WZDemo.API.Data
{
	public class WZDemoDBContext : DbContext
	{
		public WZDemoDBContext(DbContextOptions<WZDemoDBContext> dbContextOptions) : base(dbContextOptions)
		{

		}

		public DbSet<Difficulty> Difficulties { get; set; }
		public DbSet<Region> Regions { get; set; }
		public DbSet<Walk> Walks { get; set; }
		public DbSet<Image> Images { get; set; }

		// 可在 Entity Framework Core (EF Core) 構建模型時進行自定義配置。
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// seed data for Difficulties
			// easy, medium, hard

			//Seed difficulty to the databse
			var difficulty = new List<Difficulty>() {
				new Difficulty(){
				 Id = Guid.Parse("6ad20ad4-6d2d-42f1-9a02-5477719a73c0"),
				 Name = "Easy",
				},
				new Difficulty(){
				 Id = Guid.Parse("7320a35b-0e7f-4d3d-8fe0-21da846dc1f5"),
				 Name = "Medium",
				},
				new Difficulty(){
				 Id = Guid.Parse("e7996ade-e2c3-48ee-8c70-964d50dc8726"),
				 Name = "Hard",
				}
			};
			modelBuilder.Entity<Difficulty>().HasData(difficulty);

			// seed data for Regions
			var regions = new List<Region>() {
				new Region(){
				 Id = Guid.Parse("bfcf56fa-243b-49a8-874c-50eb09f674b9"),
				 Code = "NW",
				 Name = "North West",
				 RegionImageUrl = "https://www.walkinginengland.co.uk/northwest.jpg"
				},
				new Region(){
				 Id = Guid.Parse("7ee88bf5-db05-4351-adf3-e91f24bb5a90"),
				 Code = "NE",
				 Name = "North East",
				 RegionImageUrl = "https://www.walkinginengland.co.uk/northeast.jpg"
				},
				new Region(){
				 Id = Guid.Parse("00ba179f-f1da-45fd-8a48-b830504ab3f1"),
				 Code = "SW",
				 Name = "South West",
				 RegionImageUrl = "https://www.walkinginengland.co.uk/southwest.jpg"
				}
			};
			modelBuilder.Entity<Region>().HasData(regions);
		}
	}
}
