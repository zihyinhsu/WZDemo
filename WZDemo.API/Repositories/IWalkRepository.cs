﻿using WZDemo.API.Models.Domain;
using WZDemo.API.Models.DTOs;

namespace WZDemo.API.Repositories
{
	public interface IWalkRepository
	{
		Task<Walk> CreateAsync(Walk walk);
		Task<List<Walk>> GetAllAsync(
			string? filterOn, string? filterQuery,string? sortBy,bool isAscending,
			int pageNumber, int pageSize
			);
		Task<Walk?> GetByIdAsync(Guid id);
		Task<Walk?> UpdateAsync(Guid id, Walk walk);
		Task<Walk?> DeleteAsync(Guid id);
	}
}
