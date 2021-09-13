using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityGuide.API.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CityGuide.API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace CityGuide.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings").GetSection("Token").Value);

			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					p => p.AllowAnyOrigin()
						.AllowAnyHeader()
						.AllowAnyMethod());
			});

			services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

			services.AddDbContext<DataContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("Development")));

			var mapperConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new Helpers());
			});

			services.AddControllers().AddNewtonsoftJson(options =>
				options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			);
			IMapper mapper = mapperConfig.CreateMapper();
			services.AddSingleton(mapper);

			services.AddScoped<IAppRepository, AppRepository>();
			services.AddScoped<IAuthRepository, AuthRepository>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});
			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors("AllowAll");

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
