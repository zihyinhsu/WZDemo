using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using WZDemo.API.CustomActionFilter;
using WZDemo.API.Models.Domain;
using WZDemo.API.Models.DTOs;
using WZDemo.API.Repositories;

namespace WZDemo.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WalksController : ControllerBase
	{
		private readonly IMapper mapper;
		private readonly IWalkRepository walkRepository;

		public WalksController(IMapper mapper, IWalkRepository walkRepository)
		{
			this.mapper = mapper;
			this.walkRepository = walkRepository;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
		{

			// Map Dto to Domain Model
			var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);
			await walkRepository.CreateAsync(walkDomainModel);

			// Map Domain Model to Dto
			var walkDto = mapper.Map<WalkDto>(walkDomainModel);
			return Ok(walkDto);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll(
			[FromQuery] string? filterOn,
			[FromQuery] string? filterQuery,
			[FromQuery] string? sortBy,
			[FromQuery] bool? isAscending,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 1000
			)
		{
			var walksDomainModel = await walkRepository.GetAllAsync(
				filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);
			return Ok(mapper.Map<List<WalkDto>>(walksDomainModel));
		}

		[HttpGet]
		[Route("{id:Guid}")]
		[ValidateModel]
		public async Task<IActionResult> GetById([FromRoute] Guid id)
		{
			var walkDomainModel = await walkRepository.GetByIdAsync(id);
			if (walkDomainModel == null) return NotFound();
			return Ok(mapper.Map<WalkDto>(walkDomainModel));
		}

		[HttpPut]
		[Route("{id:Guid}")]
		[ValidateModel]
		public async Task<IActionResult> Update(
			[FromRoute] Guid id,
			[FromBody] UpdateWalkRequestDto updateWalkRequestDto)
		{

			var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);
			walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);

			if (walkDomainModel == null) return NotFound();

			return Ok(mapper.Map<WalkDto>(walkDomainModel));
		}

		[HttpDelete]
		[Route("{id:Guid}")]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			var walkDomainModel = await walkRepository.DeleteAsync(id);
			if (walkDomainModel == null) return NotFound();
			return Ok(mapper.Map<WalkDto>(walkDomainModel));
		}
	}
}
