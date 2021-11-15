using API.Entities;
using API.Helpers;
using API.Models;
using System.Threading.Tasks;

namespace API.Interfaces.Repository
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int soucerUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeModel>> GetUserLikes(LikesParams likesParams);
        Task RemoveLike(UserLike userLike);
    }
}
