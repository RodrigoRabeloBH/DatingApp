using System.Threading.Tasks;
using API.Entities;
using API.Models;

namespace API.Interfaces
{
    public interface IAccountServices
    {
        Task<AppUser> Register(RegisterModel model);
        Task<bool> Login(AppUser user, LoginModel model);
        Task<AppUser> UserExists(string username);
        string CreateToken(AppUser user);
    }
}