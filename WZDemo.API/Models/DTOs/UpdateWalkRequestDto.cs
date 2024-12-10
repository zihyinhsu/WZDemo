﻿using System.ComponentModel.DataAnnotations;

namespace WZDemo.API.Models.DTOs
{
	public class UpdateWalkRequestDto
	{
		[Required]
		[MaxLength(100)]
		public string Name { get; set; }
		[Required]
		[MaxLength(1000)]
		public string Description { get; set; }
		[Required]
		[Range(0,50)]
		public int LengthInKm { get; set; }
		public string WalkImageUrl { get; set; }
		[Required]
		public Guid DifficultyId { get; set; }
		[Required]
		public Guid RegionId { get; set; }
	}
}
