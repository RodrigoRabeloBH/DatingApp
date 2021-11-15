using API.Helpers;
using API.Models;
using System.Threading.Tasks;

namespace API.Interfaces.Services
{
    public interface ILikeServices
    {
        Task AddLike(int sourceUserId, string likedUsername);

        Task<PagedList<LikeModel>> GetUserLikes(LikesParams likesParams);
    }
}
