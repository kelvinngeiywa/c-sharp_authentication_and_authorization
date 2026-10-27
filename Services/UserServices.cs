using Auth.Data;
using Auth.Models;
using Auth.Models.DTOs;
using Auth.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Services
{
    public class UserServices(AppDbContext _dbContext,IConfiguration configuration) : IUser
    {
        public async Task<string> LoginUserAsync(UserDTO loginRequest)
        {
            var user = await _dbContext.users.FirstOrDefaultAsync(user=>user.Username == loginRequest.Username);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            string token = CreateToken(user);
            return CreateToken(user);
        }

        //public async Task<User> RegisterUserAsync(UserDTO registerUser)
        //{
        //    if(await _dbContext.users.AnyAsync(user=>user.Username == registerUser.Username))
        //    {
        //        return null;
        //    }
        //    var hashedPassword = new PasswordHasher<User>().HashPassword(registerUser, registerUser.Password);

        //    registerUser.PasswordHash= hashedPassword;

        //    _dbContext.users.Add(registerUser);
        //    await _dbContext.SaveChangesAsync();

        //    return registerUser;
        //}
        public async Task<User> RegisterUserAsync(UserDTO registerUser)
        {
            if (await _dbContext.users.AnyAsync(user => user.Username == registerUser.Username))
            {
                return null;
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, registerUser.Password);

            user.Username= registerUser.Username;
            user.PasswordHash = hashedPassword;
            user.Role= registerUser.Role;

            _dbContext.users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                audience: configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
