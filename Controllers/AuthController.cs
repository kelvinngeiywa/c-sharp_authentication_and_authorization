using Auth.Models;
using Auth.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
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

            string token = "Success!!";
            return Ok(token);
        }
    }
}
