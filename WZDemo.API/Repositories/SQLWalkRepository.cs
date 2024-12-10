using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WZDemo.API.Data;
using WZDemo.API.Models.Domain;
using WZDemo.API.Models.DTOs;

namespace WZDemo.API.Repositories
{
	public class SQLWalkRepository : IWalkRepository
	{
		private readonly WZDemoDBContext dbContext;

		public SQLWalkRepository(WZDemoDBContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task<Walk> CreateAsync(Walk walk)
		{
			await dbContext.Walks.AddAsync(walk);
			await dbContext.SaveChangesAsync();
			return walk;
		}


		public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
			string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
		{
			var walks = dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

			// Filtering
			if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
			{

				if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
				{

					walks = walks.Where(w => w.Name.Contains(filterQuery));
				}
			}

			// Sorting
			if (!string.IsNullOrWhiteSpace(sortBy))
			{
				if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
				{
					walks = isAscending ? walks.OrderBy(w => w.Name) : walks.OrderByDescending(w => w.Name);
				}
				else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
				{
					walks = isAscending ? walks.OrderBy(w => w.LengthInKm) : walks.OrderByDescending(w => w.LengthInKm);
				}
			}

			// Pagination
			var skipResults = (pageNumber -1) * pageSize;

			return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
		}

		public async Task<Walk?> GetByIdAsync(Guid id)
		{
			return await dbContext.Walks
						.Include("Difficulty")
						.Include("Region")
						.FirstOrDefaultAsync(w => w.Id == id);
		}

		public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
		{
			var existingWalk = await GetByIdAsync(id);
			if (existingWalk == null) return null;

			existingWalk.Name = walk.Name;
			existingWalk.Description = walk.Description;
			existingWalk.LengthInKm = walk.LengthInKm;
			existingWalk.WalkImageUrl = walk.WalkImageUrl;
			existingWalk.DifficultyId = walk.DifficultyId;
			existingWalk.RegionId = walk.RegionId;

			await dbContext.SaveChangesAsync();

			return existingWalk;
		}

		public async Task<Walk?> DeleteAsync(Guid id)
		{
			var existing = await GetByIdAsync(id);
			if (existing == null) return null;

			dbContext.Remove(existing);
			await dbContext.SaveChangesAsync();
			return existing;
		}
	}
}
