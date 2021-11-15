using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Models;

namespace API.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<AppUser>
    {
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<AppUser> GetUserByIdAsync(int id);
        Task<MemberModel> GetMemberAsync(string username);
        Task<PagedList<MemberModel>> GetMembersAsync(UserParams userParams);
    }
}