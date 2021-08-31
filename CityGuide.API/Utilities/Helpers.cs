using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityGuide.API.Dtos;
using CityGuide.API.Models;

namespace CityGuide.API.Utilities
{
	public class Helpers:Profile
	{
		public Helpers()
		{
			CreateMap<City, CityForListDto>()
				.ForMember(dest=>dest.PhotoUrl, opt =>
				{
					opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);
				});

			CreateMap<City, CityForDetailDto>();
			CreateMap<Photo, PhotoForCreationDto>();
			CreateMap<PhotoForReturnDto, Photo>();
		}
	}
}
