using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WZDemo.API.CustomActionFilter;
using WZDemo.API.Data;
using WZDemo.API.Models;
using WZDemo.API.Models.Domain;
using WZDemo.API.Models.DTOs;
using WZDemo.API.repositories;

namespace WZDemo.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RegionsController : ControllerBase
	{
		private readonly WZDemoDBContext dbContext;
		private readonly IRegionRepository regionRepository;
		private readonly IMapper mapper;

		public RegionsController(
			WZDemoDBContext dbContext,
			IRegionRepository regionRepository,
			IMapper mapper)
		{
			this.dbContext = dbContext;
			this.regionRepository = regionRepository;
			this.mapper = mapper;
		}


		[HttpGet]
		[Authorize(Roles ="Reader,Writer")]
		public async Task<IActionResult> GetAllRegion()
		{
			// get Data from database
			var regionsDomain = await regionRepository.GetAllAsync();

			// Map Domain To DTOs
			var regionDto = mapper.Map<List<RegionDto>>(regionsDomain);

			return Ok(regionDto);
		}

		[HttpGet]
		[Authorize(Roles = "Reader,Writer")]
		[Route("{id:Guid}")]
		public async Task<IActionResult> GetById([FromRoute] Guid id)
		{
			var regionsDomain = await regionRepository.GetByIdAsync(id);

			if (regionsDomain == null)
			{
				return NotFound();
			}

			// Convert domain to dto	
			var regionDto = mapper.Map<RegionDto>(regionsDomain);

			return Ok(regionDto);
		}

		[HttpPost]
		[Authorize(Roles = "Writer")]
		[ValidateModel]
		public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
		{
			// Map Dto to Model
			var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

			regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

			// Map Model to Dto
			var regionDto = mapper.Map<RegionDto>(regionDomainModel);

			return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
		}

		[HttpPut]
		[Route("{id:Guid}")]
		[ValidateModel]
		[Authorize(Roles = "Writer")]
		public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
		{
			// Map Dto to Model
			var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

			regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

			if (regionDomainModel == null) return NotFound();

			// Convert domain to dto
			var regionDto = mapper.Map<RegionDto>(regionDomainModel);

			return Ok(regionDto);
		}

		[HttpDelete]
		[Authorize(Roles = "Writer")]
		[Route("{id:Guid}")]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			var regionDomainModel = await regionRepository.DeleteAsync(id);

			if (regionDomainModel == null)
			{
				return NotFound();
			}

			// Map Model To Dto
			var regionDto = mapper.Map<RegionDto>(regionDomainModel);
			return Ok(regionDto);
		}
	}
}
