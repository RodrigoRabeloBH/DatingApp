using API.Entities;
using API.Helpers;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces.Repository
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int soucerUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeModel>> GetUserLikes(LikesParams likesParams);
        Task RemoveLike(UserLike userLike);
        Task<IEnumerable<UserLike>> GetLikes(int sourceUserId);
        Task<bool> RemoveAllLikes(IEnumerable<UserLike> likes);
    }
}
