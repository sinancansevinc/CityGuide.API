using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CityGuide.API.Data;
using CityGuide.API.Dtos;
using CityGuide.API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CityGuide.API.Controllers
{
	[Route("api/Auth")]
	[Produces("application/json")]
	public class AuthController : ControllerBase
	{
		private IAuthRepository _authRepository;
		private IConfiguration _configuration;

		public AuthController(IAuthRepository authRepository, IConfiguration configuration)
		{
			_authRepository = authRepository;
			_configuration = configuration;
		}

		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)
		{
			if (await _authRepository.UserExists(userForRegisterDto.UserName))
			{
				ModelState.AddModelError("Username","Username already exists");
				return BadRequest(ModelState);
			}

			var userCreate = new User
			{
				UserName = userForRegisterDto.UserName,
			};

			var createdUser=_authRepository.Register(userCreate, userForRegisterDto.Password);

			return StatusCode(201,createdUser);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
		{
			var user = await _authRepository.Login(userForLoginDto.UserName, userForLoginDto.Password);

			if (user == null)
			{
				return Unauthorized();
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings").GetSection("Token").Value);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Name, user.UserName)
				}),
				Expires = DateTime.Now.AddDays(1),
				SigningCredentials =
					new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			return Ok(tokenString);

		}


	}
}
