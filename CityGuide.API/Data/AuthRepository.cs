using CityGuide.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CityGuide.API.Data
{
	public class AuthRepository : IAuthRepository
	{
		private DataContext _context;

		public AuthRepository(DataContext context)
		{
			_context = context;
		}

		public async Task<User> Register(User user, string password)
		{
			byte[] passwordHash, passwordSalt;
			CreatePasswordHash(password, out passwordHash, out passwordSalt);

			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;


			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();


			return user;

		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

			}
		}

		public async Task<bool> UserExists(string username)
		{
			if (await _context.Users.AnyAsync(p => p.UserName == username))
			{
				return true;
			}

			return false;
		}

		public async Task<User> Login(string userName, string password)
		{
			var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

			if (user == null)
				return null;

			if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
			{
				return null;
			}

			return user;
		}

		private bool VerifyPasswordHash(string password, byte[] userPasswordHash, byte[] userPasswordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512(userPasswordSalt))
			{
				var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

				for (int i = 0; i < computeHash.Length; i++)
				{
					if (computeHash[i] != userPasswordHash[i])
					{
						return false;
					}
				}

				return true;

			}
		}
	}
}
