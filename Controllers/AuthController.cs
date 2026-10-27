using Auth.Models;
using Auth.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        public static User user = new();
        [HttpPost("register")]
        public ActionResult<User> RegisterUser(UserDTO newUser)
        {
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, newUser.Password);

            user.Username= newUser.Username;
            user.PasswordHash=hashedPassword;

            return Ok(user);

        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDTO loginrequest)
        {
            if(user.Username != loginrequest.Username)
            {
                return BadRequest("Username not found");
            }
            if(new PasswordHasher<User>().VerifyHashedPassword(user,user.PasswordHash,loginrequest.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest("Password is incorrect");
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Name,user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                audience:configuration.GetValue<string>("Jwt:Audience"),
                claims:claims,
                expires:DateTime.UtcNow.AddHours(5),
                signingCredentials:creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
