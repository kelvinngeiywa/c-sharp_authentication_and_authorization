using Auth.Models;
using Auth.Models.DTOs;
using Auth.Services.IServices;
using Microsoft.AspNetCore.Authorization;
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
    public class AuthController(IUser _user) : ControllerBase
    {
        //[HttpPost("register")]
        //public async Task<ActionResult<User>> RegisterUser(UserDTO newUser)
        //{
        //    var addedUser= new User{ 
        //        Username=newUser.Username,
        //        PasswordHash=newUser.Password
        //    };
        //    var user = await _user.RegisterUserAsync(addedUser);

        //    if(user is null)
        //    {
        //        return BadRequest("Username already exists");
        //    }

        //    return Ok(user);

        //}
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(UserDTO newUser)
        {
          
            var user = await _user.RegisterUserAsync(newUser);

            if (user is null)
            {
                return BadRequest("Username already exists");
            }

            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO loginrequest)
        {
            var userToken= await _user.LoginUserAsync(loginrequest);
            if (userToken is null)
            {
                return BadRequest("Invalid username or password");
            }
            return Ok(userToken);
        }

        [Authorize]
        [HttpGet]
        public ActionResult<string> AuthenticatedUsersOnly()
        {
            return Ok("You are authenticated!!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public ActionResult<string> AuthorizedOnly()
        {
            return Ok("You are authorised to access this!!");
        }

        //private string CreateToken(User user)
        //{
        //    var claims = new List<Claim> 
        //    { 
        //        new Claim(ClaimTypes.Name,user.Username)
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Token")!));

        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        //    var tokenDescriptor = new JwtSecurityToken(
        //        issuer: configuration.GetValue<string>("Jwt:Issuer"),
        //        audience:configuration.GetValue<string>("Jwt:Audience"),
        //        claims:claims,
        //        expires:DateTime.UtcNow.AddHours(5),
        //        signingCredentials:creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        //}
    }
}
