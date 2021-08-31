using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityGuide.API.Data;
using CityGuide.API.Dtos;
using CityGuide.API.Models;

namespace CityGuide.API.Controllers
{
	[Route("api/Cities")]
	public class CitiesController : Controller
	{
		private IAppRepository _repository;
		private IMapper _mapper;

		public CitiesController(IAppRepository repository,IMapper mapper)
		{
			_repository = repository;
			_mapper = mapper;
		}

		public IActionResult GetCities()
		{
			var cities = _repository.GetCities();
			var citiesToReturn = _mapper.Map<List<CityForListDto>>(cities);
			return Ok(citiesToReturn);
		}

		[HttpPost]
		[Route("add")]
		public IActionResult Add([FromBody]City city)
		{
			_repository.Add(city);
			_repository.SaveAll();
			return Ok(city);
		}

		[HttpGet]
		[Route("detail")]
		public IActionResult GetCityById(int id)
		{
			var city = _repository.GetCityById(id);
			var cityToReturn = _mapper.Map<CityForDetailDto>(city);

			return Ok(cityToReturn);
		}

		[HttpGet]
		[Route("photos")]
		public IActionResult GetPhotosByCity(int cityId)
		{
			var photos = _repository.GetPhotosByCity(cityId);
			return Ok(photos);
		}
	}
}
