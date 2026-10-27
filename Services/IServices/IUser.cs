using Auth.Models;
using Auth.Models.DTOs;

namespace Auth.Services.IServices
{
    public interface IUser
    {
        //Task<User> RegisterUserAsync(User registerUser);
        Task<User> RegisterUserAsync(UserDTO registerUser);
        Task<string> LoginUserAsync(UserDTO loginRequest);
    }
}
