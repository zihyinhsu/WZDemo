using Microsoft.AspNetCore.Mvc;
using WZDemo.API.Models.Domain;
using WZDemo.API.Models.DTOs;

namespace WZDemo.API.repositories
{
	public interface IRegionRepository
	{
		Task<List<Region>> GetAllAsync();
		Task<Region?> GetByIdAsync(Guid id);
		Task<Region> CreateAsync(Region region);
		Task<Region?> UpdateAsync(Guid id, Region region);
		Task<Region?> DeleteAsync(Guid id);
	}
}
