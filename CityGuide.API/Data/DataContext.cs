using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityGuide.API.Models;

namespace CityGuide.API.Data
{
	public class DataContext:DbContext
	{
		public DataContext(DbContextOptions<DataContext> options):base(options)
		{
				
		}

		public DbSet<City> Cities { get; set; }
		public DbSet<Photo> Photos { get; set; }
		public DbSet<User> Users { get; set; }
		
	}
}
