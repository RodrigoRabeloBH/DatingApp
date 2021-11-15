using API.Helpers;
using API.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Interfaces.Services
{
    public interface IUserServices
    {
        Task<bool> RemovePhoto(string username, int photoId);
        Task<bool> SetMainPhoto(string username, int photoId);
        Task<PhotoModel> AddPhoto(string username, IFormFile file);
        Task<bool> UpdateUser(string username, MemberUpdateModel model);
        Task<MemberModel> GetByUsername(string username);
        Task<PagedList<MemberModel>> GetAllMembers(UserParams userParams, string username);
        Task<bool> RemoveUser(string username);
    }
}
