using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WZDemo.API.Data;
using WZDemo.API.Models.Domain;
using WZDemo.API.Models.DTOs;

namespace WZDemo.API.repositories
{
	public class SQLRegionRepository : IRegionRepository
	{
		private readonly WZDemoDBContext dbContext;

		public SQLRegionRepository(WZDemoDBContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<Region> CreateAsync(Region region)
		{
			await dbContext.Regions.AddAsync(region);
			await dbContext.SaveChangesAsync();
			return region;
		}

		public async Task<List<Region>> GetAllAsync()
		{
			return await dbContext.Regions.ToListAsync();
		}

		public async Task<Region?> GetByIdAsync(Guid id)
		{
			return await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<Region?> UpdateAsync(Guid id, [FromBody] Region region)
		{
			// check if region exists
			var existingRegion = await GetByIdAsync(id);

			if (existingRegion == null) return null;

			existingRegion.Code = region.Code;
			existingRegion.Name = region.Name;
			existingRegion.RegionImageUrl = region.RegionImageUrl;

			await dbContext.SaveChangesAsync();

			return existingRegion;
		}

		public async Task<Region?> DeleteAsync(Guid id) {
			var existingRegion = await GetByIdAsync(id);

			if (existingRegion == null)
			{
				return null;
			}
			
			dbContext.Remove(existingRegion);
			await dbContext.SaveChangesAsync();

			return existingRegion;
		}
	}
}
